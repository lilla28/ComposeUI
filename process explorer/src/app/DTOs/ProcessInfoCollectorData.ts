import { ConnectionInfo } from "./ConnectionInfo";
import { ModuleInfo } from "./ModuleInfo";
import { RegistrationInfo } from "./RegistrationInfo";

export class ProcessInfoCollectorData{
    Id: number;
    Registrations: RegistrationInfo[];
    EnvironmentVariables: Map<string, string>;
    Connections: ConnectionInfo[];
    Modules: ModuleInfo[];
}