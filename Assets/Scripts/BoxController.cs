using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    
        public Transform boxContentsParent; // Kutunun içindeki nesnelerin parent objesi
        public List<GameObject> itemsInBox = new List<GameObject>();

        private void Start()
        {
            // Parent objesindeki tüm child'larý ekle
            foreach (Transform child in boxContentsParent)
            {
                itemsInBox.Add(child.gameObject);
            }
        }
    
}
