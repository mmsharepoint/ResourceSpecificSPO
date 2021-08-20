import * as React from 'react';
import * as ReactDom from 'react-dom';
import { Version } from '@microsoft/sp-core-library';
import {
  IPropertyPaneConfiguration,
  PropertyPaneTextField
} from '@microsoft/sp-property-pane';
import { BaseClientSideWebPart } from '@microsoft/sp-webpart-base';

import * as strings from 'ResourceSpecificSpoWebPartStrings';
import ResourceSpecificSpo from './components/ResourceSpecificSpo';
import { IResourceSpecificSpoProps } from './components/IResourceSpecificSpoProps';

export interface IResourceSpecificSpoWebPartProps {
  listTitle: string;
}

export default class ResourceSpecificSpoWebPart extends BaseClientSideWebPart<IResourceSpecificSpoWebPartProps> {

  public render(): void {
    const element: React.ReactElement<IResourceSpecificSpoProps> = React.createElement(
      ResourceSpecificSpo,
      {
        listTitle: this.properties.listTitle,        
        siteUrl: this.context.pageContext.site.absoluteUrl,
        serviceScope: this.context.serviceScope
      }
    );

    ReactDom.render(element, this.domElement);
  }

  protected onDispose(): void {
    ReactDom.unmountComponentAtNode(this.domElement);
  }

  protected get dataVersion(): Version {
    return Version.parse('1.0');
  }

  protected getPropertyPaneConfiguration(): IPropertyPaneConfiguration {
    return {
      pages: [
        {
          header: {
            description: strings.PropertyPaneDescription
          },
          groups: [
            {
              groupName: strings.BasicGroupName,
              groupFields: [
                PropertyPaneTextField('listTitle', {
                  label: strings.ListTitleFieldLabel
                })
              ]
            }
          ]
        }
      ]
    };
  }
}
