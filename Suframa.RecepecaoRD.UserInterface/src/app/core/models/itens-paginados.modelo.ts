export interface OpcoesPaginacao {
  page: number;
  size: number;
  sort?: string;
  reverse?: boolean;
}

export interface ItensPaginados<TItem> {
  items: TItem[];
  total: number;
}
