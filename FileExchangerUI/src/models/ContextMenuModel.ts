export class ContextMenuModel {
    text: string = "";
    thenClosed: boolean = false;
    event = (event: Event, item: ContextMenuModel) => {};
    constructor(text: string, event = (event: Event, item: ContextMenuModel) => {}, thenClosed = false) {
        this.text = text;
        this.event = event;
        this.thenClosed = thenClosed;
    }
    click(event: Event, item: ContextMenuModel): void {
        this.event(event, item);
    }
}