import { ServiceScope } from "@microsoft/sp-core-library";

export interface IResourceSpecificSpoProps {
  listTitle: string;  
  siteUrl: string;
  serviceScope: ServiceScope;
}
