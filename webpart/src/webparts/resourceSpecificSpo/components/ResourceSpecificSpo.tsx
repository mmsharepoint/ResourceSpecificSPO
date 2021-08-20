import * as React from 'react';
import jwt_decode from "jwt-decode";
import { AadHttpClientFactory, AadHttpClient, AadTokenProviderFactory, HttpClientResponse } from '@microsoft/sp-http';
import { DefaultButton } from "office-ui-fabric-react/lib/Button";
import { TextField } from "office-ui-fabric-react/lib/TextField";
import styles from './ResourceSpecificSpo.module.scss';
import { IResourceSpecificSpoProps } from './IResourceSpecificSpoProps';
const config: any = require("./azFunc.json");

const ResourceSpecificSpo: React.FunctionComponent<IResourceSpecificSpoProps> = (props) => {
  const [aadHttpClient, setAadHttpClient] = React.useState<AadHttpClient>(null);
  const [itemTitle, setItemTitle] = React.useState<string>("");
  const [isMember, setIsMember] = React.useState<boolean>(false);

  const titleChanged = React.useCallback(
    (event: React.FormEvent<HTMLInputElement | HTMLTextAreaElement>, newValue?: string) => {
      setItemTitle(newValue || '');
    },
    [],
  );

  const createItem = () => {
    aadHttpClient
      .get(`${config.hostUrl}/api/WriteListItem?url=${props.siteUrl}&listtitle=${props.listTitle}&title=${itemTitle}`, AadHttpClient.configurations.v1)
      .then((res: HttpClientResponse): Promise<any> => {
        return res.json();
      });
  };

  React.useEffect(() => {
    const factory: AadHttpClientFactory = props.serviceScope.consume(AadHttpClientFactory.serviceKey);
    const tokenFactory: AadTokenProviderFactory = props.serviceScope.consume(AadTokenProviderFactory.serviceKey);
    tokenFactory.getTokenProvider()
    .then(async (tokenProvider) => {
      const token = await tokenProvider.getToken(config.appIdUri);
      const decoded: any = jwt_decode(token);
      if (decoded.groups && decoded.groups.length > 0) {
        if (decoded.groups.indexOf(config.secGroupId) > -1) {
          setIsMember(true);
        }
      }
    });
    factory.getClient(config.appIdUri)
    .then((client) => {
      setAadHttpClient(client);
    });
  }, []);

    return (
      <div className={ styles.resourceSpecificSpo }>        
        <div className={ styles.container }>
          <div className={ styles.row }>
            <div className={ styles.column }>
              <TextField label="Item Title" value={itemTitle} maxLength={50} onChange={titleChanged} />              
            </div>
          </div>
          <div className={ styles.row }>
            <div className={ styles.column }>
              <DefaultButton onClick={createItem} disabled={!isMember} text="Create" />
            </div>
          </div>
        </div>
      </div>
    ); 
};

export default ResourceSpecificSpo;
