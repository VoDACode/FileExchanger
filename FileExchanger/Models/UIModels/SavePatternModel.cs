namespace FileExchanger.Models.UIModels
{
    public class SavePatternModel
    {
        public string Unit { get; set; }
        public float Value { get; set; }
        public float ToSecond()
        {
            if (Unit.ToUpper() == "S")
                return Value;
            else if (Unit.ToUpper() == "M")
                return Value * 60;
            else if (Unit.ToUpper() == "H")
                return Value * 3600;
            else if (Unit.ToUpper() == "D")
                return Value * 86400;
            return 0;
        }
    }
}
