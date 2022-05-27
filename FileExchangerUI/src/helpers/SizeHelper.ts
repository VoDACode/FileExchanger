export class SizeHelper {
    public static toString(size: number): string {
        let b = size;
        let kb = b / 1024;
        b %= 1024;
        let mb = kb / 1024;
        kb %= 1024;
        let gb = mb / 1024;
        mb %= 1024;
        let tb = gb / 1024;
        tb %= 1024;
        if(size / 1024 / 1024 / 1024 / 1024 >= 0.5)
            return `${Math.round(size / 1024 / 1024 / 1024 / 1024)} TB`;
        else if(size / 1024 / 1024 / 1024 >= 0.5)
            return `${Math.round(size / 1024 / 1024 / 1024)} GB`;
        else if(size / 1024 / 1024 >= 0.5)
            return `${Math.round(size / 1024 / 1024)} MB`;
        else if(size / 1024 >= 0.5)
            return `${Math.round(size / 1024)} KB`;
        else
            return `${size} B`;
    }
}