using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.zhifez.seagj {
  public class UI_Main_Services : Base {
    public Text[] labels;

    //--------------------------------------------------
    // private
    //--------------------------------------------------

    //--------------------------------------------------
    // public
    //--------------------------------------------------

    //--------------------------------------------------
    // protected
    //--------------------------------------------------
    protected void OnEnable () {
			if ( GAME == null ) {
				return;
			}

			for ( int a=0; a<DATA_PACKAGE.serviceSignalPatterns.Length; ++a ) {
				ServiceSignalPattern ssp = DATA_PACKAGE.serviceSignalPatterns[a];
				if ( !DATA_PACKAGE.ServiceIsEnabled ( ssp.service ) ) {
					continue;
				}

				labels[a].text = "<size=25>" + ssp.service + " (" + ssp.signalPatterns.Length + ")</size>";
				for ( int b=0; b<ssp.signalPatterns.Length; ++b ) {
					SignalPattern sp = ssp.signalPatterns[b];
					labels[a].text += "\n  Frequency " + b + ":";
					labels[a].text += "\n    signal strength: " + sp.strength;
					labels[a].text += "\n    signal speed: " + sp.speed;
				}
			}
    }
  }
}