using System.Collections.Generic;


namespace Ferrum.Player
{
    class ItemTypeDataset : Dataset
    {
        public override string searchCategory { get { return "itemTypes"; } }

        // Dictionary to hold the Dataset
        public List<InventoryItemType> itemTypes = new();


        public override bool Has(string id)
        {
            // This function is required always
            return GetItemType(id) != null;
        }

        // Name this function whatever you want
        public InventoryItemType GetItemType(string id)
        {
            return itemTypes.Find(x => x?.id == id);
        }

        public static InventoryItemType Get(string id)
        {
            return (GetDS(id, "itemTypes") as ItemTypeDataset).GetItemType(id);
        }
    }
}