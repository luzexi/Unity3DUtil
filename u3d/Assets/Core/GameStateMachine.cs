using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#pragma warning disable 162

#region statemachine
//namespace ROCKETPACK_PANDA
//{
//Delegates
public delegate void OnEnterHandler();
public delegate void OnUpdateHandler();
public delegate void OnExitHandler();



public class GameStateMachine<T> where T : struct
{
    public class GameState
    {
        public T m_stateName = default(T);//"DefaultStateName";

        //It is the FSM the state inside
        public GameStateMachine<T> m_fsm = null;

        //Time based variables
        public bool m_isTimeBased = false;
        public GameState m_nextState = null;
        protected float m_stateDuration = 0;
        protected float m_curTime = 0;

        //Events
        public event OnEnterHandler OnEnter;
        public event OnUpdateHandler OnUpdate;
        public event OnExitHandler OnExit;

        public GameState()
        {
            m_isTimeBased = false;
            m_stateDuration = 0;
            m_curTime = 0;
            m_nextState = null;
        }

        public GameState(GameStateMachine<T> stateMachine, T stateName)
        {
            m_fsm = stateMachine;
            m_stateName = stateName;
            m_isTimeBased = false;
            m_stateDuration = 0;
            m_curTime = 0;
            m_nextState = null;
        }

        public void SetInfo(GameStateMachine<T> stateMachine, T stateName)
        {
            m_fsm = stateMachine;
            m_stateName = stateName;
        }

        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void Exit() { }

        public virtual void EnterState()
        {
            m_curTime = 0;

            if (OnEnter != null)
            {
                OnEnter();
            }
            Enter();
        }

        public virtual void Execute()
        {
            m_curTime += Time.deltaTime;
            if (m_isTimeBased && m_curTime >= m_stateDuration)
            {
                if (m_nextState != null)
                {
                    m_fsm.ChangeState(m_nextState.m_stateName);
                }
                return;
            }

            if (OnUpdate != null)
            {
                OnUpdate();
            }
            Update();
        }

        public virtual void ExitState()
        {
            if (OnExit != null)
            {
                OnExit();
            }
            Exit();
        }
    }

    public Dictionary<T, GameState> m_stateList = new Dictionary<T, GameState>();

    public GameState m_curState { get; private set; }
    public GameState m_preState { get; private set; }

    public GameState this[T stateName]
    {
        get
        {
            if (!m_stateList.ContainsKey(stateName))
                return null;

            return m_stateList[stateName];
        }
        set
        {
            m_stateList[stateName] = value;
        }
    }

    public GameStateMachine()
    {
//        if (!typeof(T).IsEnum)
//        {
//            throw new ArgumentException("T must be an enumerated type");
//        }
    }

    public void AddState(T stateName)
    {
        m_stateList.Add(stateName, new GameState(this, stateName));
    }
    public void AddState(T stateName, GameState state)
    {
        state.m_fsm = this;
        state.m_stateName = stateName;
        m_stateList.Add(stateName, state);
    }

    public void ChangeState(T newStateName)
    {
        if (!m_stateList.ContainsKey(newStateName))
        {
#if UNITY_EDITOR
            Debug.Log("Error! No State named :" +newStateName.ToString());
            throw new UnityException();
#endif
            return;
        }

        GameState newState = m_stateList[newStateName];
        if (newState == null)
            Debug.LogError("FSM Error could not change to state:" + newStateName);

        if (newState == null) return;

        //set fsm to state
        newState.m_fsm = this;

        //Exit current state first
        if (m_curState != null) m_curState.ExitState();

        //record previous state
        m_preState = m_curState;

        //Change currnet state
        m_curState = newState;

        //Entry new state
        m_curState.EnterState();
    }

    public T GetCurrentStateName()
    {
        return m_curState.m_stateName;
    }

    public GameState GetCurrentState()
    {
        if(m_curState == null) return null;
        
        return m_stateList[m_curState.m_stateName];
    }

    public bool IsInState(GameState checkState)
    {
        return m_curState == checkState;
    }

    public virtual void Update()
    {
        if (m_curState == null) return;

        //excute current state
        m_curState.Execute();
    }
}
//}
#endregion
#region stepmachine
class GameStepMachine
{
    System.Action m_callback = null;
    List<GameStep> m_steps = new List<GameStep>();
    int m_curStep = 0;

    public GameStepMachine()
    {
    }
    public void AddStep(GameStep _step)
    {
        m_steps.Add(_step);
    }
    public void Start(System.Action _callback = null)
    {
        m_callback = _callback;

        m_curStep = 0;

        if (m_curStep < m_steps.Count)
        {
            m_steps[m_curStep].EnterStep();
        }
    }
    public void Update()
    {
        if (m_steps != null && m_curStep < m_steps.Count)
        {
            GameStep.UpdateStepResult result = m_steps[m_curStep].UpdateStep();
            if (result == GameStep.UpdateStepResult.Next)
            {
                m_steps[m_curStep].ExitStep();
                ++m_curStep;
                if (m_curStep < m_steps.Count)
                {
                    m_steps[m_curStep].EnterStep();
                }
                else if (m_callback != null)
                {
                    m_callback();
                }
            }
        }
    }
}
abstract class GameStep
{
    public enum UpdateStepResult
    {
        Stay,
        Next,
    }
    public abstract void EnterStep();
    public abstract UpdateStepResult UpdateStep();
    public abstract void ExitStep();

}
#endregion