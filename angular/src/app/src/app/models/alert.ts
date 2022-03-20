export interface Alert {
    type: AlertType;
    message: string;
    autoClose: boolean;
}

export enum AlertType {
    Success,
    Error,
    Info,
    Warning
}