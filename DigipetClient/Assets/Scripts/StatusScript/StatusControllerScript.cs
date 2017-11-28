using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusControllerScript : MonoBehaviour {

    public GameObject petName;
    public GameObject age;
    public GameObject energySlider;
    public GameObject hungerSlider;
    public GameObject funSlider;
    public GameObject hygieneSlider;
    public GameObject environmentSlider;

	// Use this for initialization
	void Start () {
        petName = GameObject.Find("PetNameValue");
        age = GameObject.Find("AgeValue");
        energySlider = GameObject.Find("EnergySlider");
        hungerSlider = GameObject.Find("HungerSlider");
        funSlider = GameObject.Find("FunSlider");
        hygieneSlider = GameObject.Find("HygieneSlider");
        environmentSlider = GameObject.Find("EnvironmentSlider");

        petName.GetComponent<Text>().text = DataControllerScript.dataController.petName;
        age.GetComponent<Text>().text = DataControllerScript.dataController.age + " years old";
        energySlider.GetComponent<Slider>().value = (float)DataControllerScript.dataController.energy;
        hungerSlider.GetComponent<Slider>().value = (float)DataControllerScript.dataController.hunger;
        funSlider.GetComponent<Slider>().value = (float)DataControllerScript.dataController.fun;
        hygieneSlider.GetComponent<Slider>().value = (float)DataControllerScript.dataController.hygiene;
        environmentSlider.GetComponent<Slider>().value = (float)DataControllerScript.dataController.environment;
    }
	
	// Update is called once per frame
	void Update () {
        energySlider.GetComponent<Slider>().value = (float)DataControllerScript.dataController.energy;
        hungerSlider.GetComponent<Slider>().value = (float)DataControllerScript.dataController.hunger;
        funSlider.GetComponent<Slider>().value = (float)DataControllerScript.dataController.fun;
        hygieneSlider.GetComponent<Slider>().value = (float)DataControllerScript.dataController.hygiene;
        environmentSlider.GetComponent<Slider>().value = (float)DataControllerScript.dataController.environment;
    }
}
