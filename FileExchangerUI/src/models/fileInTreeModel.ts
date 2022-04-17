export class FileTreeModel{
    public name: string = "Name";
    public key: string = "key";
    public isHaveFolders: boolean = false;
    public root: FileTreeModel | undefined;
}