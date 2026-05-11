export type AmbienteApp = Readonly<{
  production: boolean;
  developmentMode: boolean;
  serviceUrl: string;
}>;