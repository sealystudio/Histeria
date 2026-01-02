using System;
using System.Collections.Generic;
using UnityEngine;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.UnityToolkit;
using BehaviourAPI.UtilitySystems;

public class US_code_EliCorrupta : BehaviourRunner
{
	[SerializeField] private EliCorrupta m_EliCorrupta;
	
	protected override void Init()
	{
		m_EliCorrupta = GetComponent<EliCorrupta>();
		
		base.Init();
	}
	
	protected override BehaviourGraph CreateGraph()
	{
		UtilitySystem US_Eli_Corrupta = new UtilitySystem(1.3f);
		
		VariableFactor Distancia_Juagdor = US_Eli_Corrupta.CreateVariable(m_EliCorrupta.DistanciaJugador, 0f, 1000000f);
		
		VariableFactor Vida = US_Eli_Corrupta.CreateVariable(m_EliCorrupta.VidaActual, 0f, 3f);
		
		LinearCurveFactor Distancia_Idle = US_Eli_Corrupta.CreateCurve<LinearCurveFactor>(Distancia_Juagdor);
		
		SimpleAction Idle_action = new SimpleAction();
		Idle_action.action = m_EliCorrupta.Idle;
		UtilityAction Idle = US_Eli_Corrupta.CreateAction(Distancia_Idle, Idle_action);
		
		LinearCurveFactor Distancia_Ataque = US_Eli_Corrupta.CreateCurve<LinearCurveFactor>(Distancia_Juagdor);
		
		SimpleAction DisparoEspejo_action = new SimpleAction();
		UtilityAction DisparoEspejo = US_Eli_Corrupta.CreateAction(Distancia_Ataque, DisparoEspejo_action);
		
		SigmoidCurveFactor vida_a_1 = US_Eli_Corrupta.CreateCurve<SigmoidCurveFactor>(Vida);

        MaxFusionFactor Maximo = US_Eli_Corrupta.CreateFusion<MaxFusionFactor>(Distancia_Ataque, vida_a_1);

        SimpleAction AreaDamage_action = new SimpleAction();
		AreaDamage_action.action = m_EliCorrupta.CargarAtaqueArea;
		UtilityAction AreaDamage = US_Eli_Corrupta.CreateAction(Maximo, AreaDamage_action);
		
		return US_Eli_Corrupta;
	}
}
