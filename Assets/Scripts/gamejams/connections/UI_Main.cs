using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace com.zhifez.seagj {
  public class UI_Main : Base {
    public static UI_Main instance;

    public Transform container;
    public Text title;
    public Text funds;
    public GameObject[] sectionGOs;

    public enum Section {
      transmission,
      connections,
      services,
      guide,
      about
    }
    private string[] sectionNames {
      get { return System.Enum.GetNames ( typeof ( Section ) ); }
    }
    private Section _currentSection = Section.transmission;
    private Section currentSection {
      get { return _currentSection; }
      set {
        _currentSection = value;

        title.text = "";
        for ( int a=0; a<sectionNames.Length; ++a ) {
          bool _isActive = ( ( Section ) a == _currentSection );
          sectionGOs[a].SetActive ( _isActive );

          switch ( a ) {
          case 0:
            UI_MAIN_TRANSMISSION.ClearLabels ();
            break;
          }

          if ( a > 0 ) {
            title.text += " | ";
          }
          if ( _isActive ) {
            title.text += "<b>" + sectionNames[a] + "</b>";
          }
          else {
            title.text += "<size=30>" + sectionNames[a] + "</size>";
          }
        }
      }
    }

    //--------------------------------------------------
    // private
    //--------------------------------------------------

    //--------------------------------------------------
    // public
    //--------------------------------------------------
    public void GoToPrevSection () {
      int _section = ( int ) currentSection;
      --_section;
      if ( _section <= 0 ) {
        _section = 0;
      }
      currentSection = ( Section ) _section;
    }

    public void GoToNextSection () {
      int _section = ( int ) currentSection;
      ++_section;
      if ( _section >= sectionNames.Length ) {
        _section = sectionNames.Length - 1;
      }
      currentSection = ( Section ) _section;
    }

    public void UpdateFunds ( int amount ) {
      funds.text = "funds: $" + amount;
    }

    //--------------------------------------------------
    // protected
    //--------------------------------------------------
    protected void Awake () {
      instance = this;

      container.localScale = Vector3.zero;
      container.gameObject.SetActive ( false );
      enabled = false;
    }

    protected void Start () {
      currentSection = Section.transmission;
    }

    protected void OnDisable () {
      container.DOScale ( Vector3.zero, 0.2f )
      .SetEase ( Ease.InBack )
      .OnComplete ( () => {
        container.gameObject.SetActive ( false );
      } );
    }

    protected void OnEnable () {
      container.localScale = Vector3.zero;
      container.gameObject.SetActive ( true );
      container.DOScale ( Vector3.one, 0.2f )
      .SetEase ( Ease.OutBack );
    }

    protected void Update () {

    }
  }
}