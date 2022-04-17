using System;

namespace FileExchanger.Configs
{
    public abstract class Configurations
    {
        protected abstract dynamic ConfigSection { get; }
        protected bool ParseBool(dynamic val)
        {
            bool mode;
            if (bool.TryParse((string)val, out mode))
                return mode;
            throw new ArgumentException("");
        }
        protected T ParseEnum<T>(dynamic val)
        {
            object mode;
            if (Enum.TryParse(typeof(T), (string)val, true, out mode))
                return (T)mode;
            throw new ArgumentException("");
        }
    }
}
