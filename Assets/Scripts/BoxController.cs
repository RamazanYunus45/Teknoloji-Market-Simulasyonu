using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    
        public Transform boxContentsParent; // Kutunun i�indeki nesnelerin parent objesi
        public List<GameObject> itemsInBox = new List<GameObject>();

        private void Start()
        {
            // Parent objesindeki t�m child'lar� ekle
            foreach (Transform child in boxContentsParent)
            {
                itemsInBox.Add(child.gameObject);
            }
        }
    
}
