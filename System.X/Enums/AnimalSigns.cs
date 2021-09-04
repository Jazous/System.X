using System.ComponentModel;

namespace System.X.Enums
{
    /// <summary>
    /// 十二生肖。
    /// </summary>
    public enum AnimalSigns
    {
        [Description("鼠")]
        Rat = 1,
        [Description("牛")]
        Ox,
        [Description("虎")]
        Tiger,
        [Description("兔")]
        Rabbit,
        [Description("龙")]
        Dragon,
        [Description("蛇")]
        Snake,
        [Description("马")]
        Horse,
        [Description("羊")]
        Sheep,
        [Description("猴")]
        Monkey,
        [Description("鸡")]
        Rooster,
        [Description("狗")]
        Dog,
        [Description("猪")]
        Pig
    }
}