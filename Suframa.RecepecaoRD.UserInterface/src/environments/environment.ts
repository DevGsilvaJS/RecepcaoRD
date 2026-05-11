import { AmbienteApp } from './ambiente.app';

/** Em `ng serve` com `proxy.conf.json`, deixe vazio para chamar `/api/...` no mesmo host (4200) e o proxy encaminhar ao backend. Use `npm start` (host `localhost`) e reinicie o serve após mudar `angular.json`. */
export const environment: AmbienteApp = {
  production: false,
  developmentMode: true,
  serviceUrl: '',
};

