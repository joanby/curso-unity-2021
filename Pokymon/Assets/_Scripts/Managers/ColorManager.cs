using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ColorManager : MonoBehaviour
{
    public static ColorManager SharedInstance;
    public Color selectedColor;
    public Color defaultColor = Color.black;

    private void Awake()
    {
        SharedInstance = this;
    }
    
    
    public Color BarColor(float finalScale)
    {
        if (finalScale < 0.2f)
        {
            return new Color(193f/255, 45f/255, 45f/255);
        }else if (finalScale < 0.5f)
        {
            return new Color(211f/255, 211f/255, 29f/255);
        }
        else
        {
            return new Color(98f/255, 178f/255, 61f/255);
        }
    }
    
    public Color PPColor(float finalScale)
    {
        if (finalScale < 0.2f)
        {
            return new Color(193f/255, 45f/255, 45f/255);
        }else if (finalScale < 0.5f)
        {
            return new Color(211f/255, 211f/255, 29f/255);
        }
        else
        {
            return Color.black;
        }
    }

    public class TypeColor
    {
        private static Color[] colors =
        {
            Color.white, // None
            new Color(0.8193042f, 0.9333333f, 0.5254902f), // Bug
            new Color(0.735849f, 0.6178355f, 0.5588287f), // Dark
            new Color(0.6556701f, 0.5568628f, 0.7647059f), // Dragon
            new Color(0.9942768f, 1f, 0.5707547f), // Electric
            new Color(0.9339623f, 0.7621484f, 0.9339623f), // Fairy
            new Color(0.735849f, 0.5600574f, 0.5310609f), // Fight
            new Color(0.990566f, 0.5957404f, 0.5279903f), // Fire
            new Color(0.7358491f, 0.7708895f, 0.9811321f), // Flying
            new Color(0.6094251f, 0.6094251f, 0.7830189f), // Ghost
            new Color(0.4103774f, 1, 0.6846618f), // Grass
            new Color(0.9433962f, 0.7780005f, 0.5562478f), // Ground
            new Color(0.7216981f, 0.9072328f, 1), // Ice
            new Color(0.8734059f, 0.8773585f, 0.8235582f), // Normal
            new Color(0.6981132f, 0.4774831f, 0.6539872f), // Poison
            new Color(1, 0.6650944f, 0.7974522f), // Psychic
            new Color(0.8584906f, 0.8171859f, 0.6519669f), // Rock
            new Color(0.7889819f, 0.7889819f, 0.8490566f), // Steel
            new Color(0.5613208f, 0.7828107f, 1) // Water
        };

        public static Color GetColorFromType(PokemonType type)
        {
            return colors[(int)type];
        }
    }


    public class StatusConditionColor
    {
        //none, brn, frz, par, psn, slp
        private static Dictionary<StatusConditionID, Color> colors = new Dictionary<StatusConditionID, Color>()
        {
            {StatusConditionID.none, Color.white},
            {StatusConditionID.brn, new Color(223f / 255, 134f / 255, 67f / 255)},
            {StatusConditionID.frz, new Color(168f / 255, 214f / 255, 215f / 255)},
            {StatusConditionID.par, new Color(241f / 255, 208f / 255, 83f / 255)},
            {StatusConditionID.psn, new Color(147f / 255, 73f / 255, 156 / 255)},
            {StatusConditionID.slp, new Color(163f / 255, 147f / 255, 234f / 255)}
        };

        public static Color GetColorFromStatusCondition(StatusConditionID id)
        {
            return colors[id];
        }
    }
    
    
}
