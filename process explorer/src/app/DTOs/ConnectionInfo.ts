import { Guid } from "igniteui-angular-core";

export class ConnectionInfo{
    Id: Guid;
    Name: string;
    LocalEndpoint: string;
    RemoteEndpoint: string;
    RemoteApplication: string;
    RemoteHostname: string;
    ConnectionInformation: Map<string, string>;
    Status: string;
}