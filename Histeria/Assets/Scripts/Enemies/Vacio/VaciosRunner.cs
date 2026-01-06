using System;
using System.Collections.Generic;
using UnityEngine;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.UnityToolkit;
using BehaviourAPI.BehaviourTrees;

public class VaciosRunner : BehaviourRunner
{
    [SerializeField] private VacioController m_VacioController;

    protected override void Init()
    {
        m_VacioController = GetComponent<VacioController>();
        base.Init();
    }

    protected override BehaviourGraph CreateGraph()
    {
        BehaviourTree VaciosBT = new BehaviourTree();

        // ---------------------------------------------------------
        // 1. LAS PREGUNTAS (CHECKS) -> TIENEN QUE SER FunctionalAction
        // Porque devuelven 'Status'
        // ---------------------------------------------------------

        FunctionalAction VacioCerca_action = new FunctionalAction();
        VacioCerca_action.onUpdated = m_VacioController.CheckVacioCerca;
        LeafNode VacioCerca = VaciosBT.CreateLeafNode(VacioCerca_action);

        FunctionalAction RangoAtaque_action = new FunctionalAction();
        RangoAtaque_action.onUpdated = m_VacioController.CheckRangoAtaque;
        LeafNode RangoAtaque = VaciosBT.CreateLeafNode(RangoAtaque_action);

        FunctionalAction RangoDeteccion_action = new FunctionalAction();
        RangoDeteccion_action.onUpdated = m_VacioController.CheckRangoDeteccion;
        LeafNode RangoDeteccion = VaciosBT.CreateLeafNode(RangoDeteccion_action);

        FunctionalAction JugadorMueve_action = new FunctionalAction();
        JugadorMueve_action.onUpdated = m_VacioController.CheckJugadorMueve;
        LeafNode JugadorMueve = VaciosBT.CreateLeafNode(JugadorMueve_action);

        FunctionalAction EstaEnOrigen_action = new FunctionalAction();
        EstaEnOrigen_action.onUpdated = m_VacioController.CheckPuedePatrullar;
        LeafNode EstaEnOrigen = VaciosBT.CreateLeafNode(EstaEnOrigen_action);


        // ---------------------------------------------------------
        // 2. LAS ACCIONES (VERBOS) -> PUEDEN SER SimpleAction
        // Porque son 'void'
        // ---------------------------------------------------------

        SimpleAction Sinergia_action = new SimpleAction();
        Sinergia_action.action = m_VacioController.AplicarSinergia;
        LeafNode Sinergia = VaciosBT.CreateLeafNode(Sinergia_action);

        SimpleAction Atacar_action = new SimpleAction();
        Atacar_action.action = m_VacioController.Atacar;
        LeafNode Atacar = VaciosBT.CreateLeafNode(Atacar_action);

        SimpleAction Idle_action = new SimpleAction();
        Idle_action.action = m_VacioController.Idle;
        LeafNode Idle = VaciosBT.CreateLeafNode(Idle_action);

        SimpleAction Perseguir_action = new SimpleAction();
        Perseguir_action.action = m_VacioController.Perseguir;
        LeafNode Perseguir = VaciosBT.CreateLeafNode(Perseguir_action);

        SimpleAction Patrullar_action = new SimpleAction();
        Patrullar_action.action = m_VacioController.Patrullar;
        LeafNode Patrullar = VaciosBT.CreateLeafNode(Patrullar_action);

        SimpleAction Volver_action = new SimpleAction();
        Volver_action.action = m_VacioController.VolverAOrigen;
        LeafNode Volver = VaciosBT.CreateLeafNode(Volver_action);


        // ---------------------------------------------------------
        // 3. ESTRUCTURA DEL ÁRBOL (NO TOCA NADA AQUÍ)
        // ---------------------------------------------------------

        SequencerNode SecuencerSinergia = VaciosBT.CreateComposite<SequencerNode>(false, VacioCerca, Sinergia);
        SecuencerSinergia.IsRandomized = false;

        SuccederNode SucceederSinergia = VaciosBT.CreateDecorator<SuccederNode>(SecuencerSinergia);

        SequencerNode SecuenciaAtacar = VaciosBT.CreateComposite<SequencerNode>(false, RangoAtaque, Atacar);
        SecuenciaAtacar.IsRandomized = false;

        SuccederNode SucceederAtacar = VaciosBT.CreateDecorator<SuccederNode>(SecuenciaAtacar);

        SequencerNode SecuenciaIdle = VaciosBT.CreateComposite<SequencerNode>(false, JugadorMueve, Idle);
        SecuenciaIdle.IsRandomized = false;

        SelectorNode SelectorPerseguir = VaciosBT.CreateComposite<SelectorNode>(false, SecuenciaIdle, Perseguir);
        SelectorPerseguir.IsRandomized = false;

        SequencerNode SecuenciaDeteccion = VaciosBT.CreateComposite<SequencerNode>(false, RangoDeteccion, SelectorPerseguir);
        SecuenciaDeteccion.IsRandomized = false;

        SequencerNode SecuenciaPatrullar = VaciosBT.CreateComposite<SequencerNode>(false, EstaEnOrigen, Patrullar);
        SecuenciaPatrullar.IsRandomized = false;

        SelectorNode SelectorMovimiento = VaciosBT.CreateComposite<SelectorNode>(false, SecuenciaDeteccion, SecuenciaPatrullar, Volver);
        SelectorMovimiento.IsRandomized = false;

        SequencerNode SecuenciaPrincipal = VaciosBT.CreateComposite<SequencerNode>(false, SucceederSinergia, SucceederAtacar, SelectorMovimiento);
        SecuenciaPrincipal.IsRandomized = false;

        LoopNode LoopPrincipal = VaciosBT.CreateDecorator<LoopNode>(SecuenciaPrincipal);
        LoopPrincipal.Iterations = -1;

        return VaciosBT;
    }
}