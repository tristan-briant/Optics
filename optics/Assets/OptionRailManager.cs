using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionRailManager : OptionManager
{
   
   public override void Start(){
       base.Start();
       CP.GetComponent<RailManager>().attachMode=true;
   }

   void OnDestroy(){
       CP.GetComponent<RailManager>().attachMode=false;
   }
}
