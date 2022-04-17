export class SaveTimePattern {
    unit: "D" | "H" | "M" | "S" = "S";
    value: number = 0;
    constructor(unit: "D" | "H" | "M" | "S", value: number) {
        this.unit = unit;
        this.value = value;
    }
    public toSecond(): number {
        if (this.unit == "S")
            return this.value;
        else if (this.unit == "M")
            return this.value * 60;
        else if (this.unit == "H")
            return this.value * 3600;
        else if (this.unit == "D")
            return this.value * 86400;
        return 0;
    }
}