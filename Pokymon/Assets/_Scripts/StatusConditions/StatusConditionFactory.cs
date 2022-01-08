using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusConditionFactory
{
    public static void InitFactory()
    {
        foreach (var condition in StatusConditions)
        {
            var id = condition.Key;
            var statusCondition = condition.Value;
            statusCondition.Id = id;
        }
    }
    
    public static Dictionary<StatusConditionID, StatusCondition> StatusConditions { get; set; } =
        new Dictionary<StatusConditionID, StatusCondition>()
        {
            {
                StatusConditionID.psn,
                new StatusCondition()
                {
                    Name = "Poison",
                    Description = "Hace que el Pokemon sufra daño en cada turno.",
                    StartMessage = "ha sido envenenado",
                    OnFinishTurn = PoisonEffect
                }
            }, 
            {
                StatusConditionID.brn,
                new StatusCondition()
                {
                    Name = "Burn",
                    Description = "Hace que el Pokemon sufra daño en cada turno.",
                    StartMessage = "ha sido quemado",
                    OnFinishTurn = BurnEffect
                }
            }, 
            {
                StatusConditionID.par,
                new StatusCondition()
                {
                    Name = "Paralyzed",
                    Description = "Hace que el Pokemon pueda estar paralizado en el turno.",
                    StartMessage = "ha sido paralizado",
                    OnStartTurn = ParalyzedEffect
                }
            }, 
            {
                StatusConditionID.frz,
                new StatusCondition()
                {
                    Name = "Frozen",
                    Description = "Hace que el Pokemon esté congelado, pero se puede curar aleatoriamente durante un turno.",
                    StartMessage = "ha sido congelado",
                    OnStartTurn = FrozenEffect
                }
            }, 
            {
                StatusConditionID.slp,
                new StatusCondition()
                {
                    Name = "Sleep",
                    Description = "Hace que el Pokemon durma durante un numer fijo de turnos.",
                    StartMessage = "se ha dormido",
                    OnApplyStatusCondition = (Pokemon pokemon) =>
                    {
                        pokemon.StatusNumTurns = Random.Range(1, 4);
                        Debug.Log($"El pokemon dormirá durante {pokemon.StatusNumTurns} turnos");
                    },
                    OnStartTurn = (Pokemon pokemon) =>
                    {
                        if (pokemon.StatusNumTurns<=0)
                        {
                            pokemon.CureStatusCondition();
                            pokemon.StatusChangeMessages.Enqueue($"¡{pokemon.Base.Name} ha despertado!");
                            return true;
                        }
                        
                        pokemon.StatusNumTurns--;
                        pokemon.StatusChangeMessages.Enqueue($"{pokemon.Base.Name} sigue dormido.");
                        return false;
                    }
                }
            },
            
            /*ESTADOS VOLÁTILES A PARTIR DE AQUÍ*/
            
            {
                StatusConditionID.conf,
                new StatusCondition()
                {
                    Name = "Confusión",
                    Description = "Hace que el Pokemon esté confundido y pueda atacarse a si mismo.",
                    StartMessage = "ha sido confundido",
                    OnApplyStatusCondition = (Pokemon pokemon) =>
                    {
                        pokemon.VolatileStatusNumTurns = Random.Range(1, 6);
                        Debug.Log($"El pokemon estará confundido durante {pokemon.VolatileStatusNumTurns} turnos");
                    },
                    OnStartTurn = (Pokemon pokemon) =>
                    {
                        if (pokemon.VolatileStatusNumTurns<=0)
                        {
                            pokemon.CureVolatileStatusCondition();
                            pokemon.StatusChangeMessages.Enqueue($"¡{pokemon.Base.Name} ha salido del estado confusión!");
                            return true;
                        }
                        
                        pokemon.VolatileStatusNumTurns--;
                        pokemon.StatusChangeMessages.Enqueue($"{pokemon.Base.Name} sigue confundido.");
                        
                        if (Random.Range(0, 2) == 0)
                        {
                            return true;
                        }
                        //Debemos dañarnos a nosotros mismos por la confusión
                        pokemon.UpdateHP(pokemon.MaxHP/6);
                        pokemon.StatusChangeMessages.Enqueue("¡Tan confuso que se hiere a si mismo!");
                        return false;
                    }
                }
            }
            
        };

    static void PoisonEffect(Pokemon pokemon)
    {
        pokemon.UpdateHP(Mathf.CeilToInt((float)pokemon.MaxHP/8.0f));
        pokemon.StatusChangeMessages.Enqueue($"{pokemon.Base.Name} sufre los efectos del veneno.");
    }
    
    static void BurnEffect(Pokemon pokemon)
    {
        pokemon.UpdateHP(Mathf.CeilToInt((float)pokemon.MaxHP/15.0f));
        pokemon.StatusChangeMessages.Enqueue($"{pokemon.Base.Name} sufre los efectos de la quemadura.");
    }

    static bool ParalyzedEffect(Pokemon pokemon)
    {
        if (Random.Range(0, 100) <25)
        {
            pokemon.StatusChangeMessages.Enqueue($"{pokemon.Base.Name} está paralizado y no puede moverse.");
            return false;
        }
        return true;
    } 
    
    
    static bool FrozenEffect(Pokemon pokemon)
    {
        if (Random.Range(0, 100) <25)
        {
            pokemon.CureStatusCondition();
            pokemon.StatusChangeMessages.Enqueue($"{pokemon.Base.Name} ya no está congelado.");
            return true;
        }
        
        pokemon.StatusChangeMessages.Enqueue($"{pokemon.Base.Name} sigue congelado.");
        return false;
    }
}

public enum StatusConditionID
{
    none, brn, frz, par, psn, slp, conf
}