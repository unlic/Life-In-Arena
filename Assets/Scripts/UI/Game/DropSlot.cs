using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedItem = eventData.pointerDrag;

        if (droppedItem != null)
        {
            ItemInUI currentItemInUI = GetComponent<ItemInUI>();
            ItemInUI droppedItemInUI = droppedItem.GetComponent<ItemInUI>();

            ItemObject currentItemObject = currentItemInUI.ItemObject;
            ItemObject droppedItemObject = droppedItemInUI.ItemObject;


            int idFrom = droppedItem.transform.GetSiblingIndex();
            int idTo = transform.GetSiblingIndex();


            ItemLocation droppedLocation = droppedItemInUI.ItemLocation;
            ItemLocation currentLocation = currentItemInUI.ItemLocation;


            if(droppedLocation == currentLocation&& currentLocation == ItemLocation.Inventory)
            {
                droppedItemInUI.ChangingItemsPlaces(idTo, idFrom);
                return;
            }
            if(droppedLocation == currentLocation && currentLocation == ItemLocation.Storege)
            {
                droppedItemInUI.SetItem(currentItemObject);
                currentItemInUI.SetItem(droppedItemObject);

                return;
            }


            droppedItemInUI.SetItem(currentItemObject);
            switch (droppedLocation)
            {
                case ItemLocation.Inventory:
                    droppedItemInUI.EquipItem(idTo);
                    break;
                case ItemLocation.Storege:
                    droppedItemInUI.RequipItem();
                    break;
                case ItemLocation.None:
                    break;
                default:
                    break;
            }
            currentItemInUI.SetItem(droppedItemObject);
            switch (currentLocation)
            {
                case ItemLocation.Inventory:
                    currentItemInUI.EquipItem(idTo);
                    break;
                case ItemLocation.Storege:
                    currentItemInUI.RequipItem();
                    break;
                case ItemLocation.None:
                    break;
                default:
                    break;
            }



        }
    }

}
