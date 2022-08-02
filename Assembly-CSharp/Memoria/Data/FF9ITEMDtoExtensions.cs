namespace Global.ff9.State
{
    public static class FF9ITEMDtoExtensions
    {
        public static FF9ITEM[] ToModel(this FF9ITEMDto[] values)
        {
            FF9ITEM[] result = new FF9ITEM[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                result[i] = new FF9ITEM(values[i].ItemId, values[i].Count);
            }

            return result;
        }
    }
}