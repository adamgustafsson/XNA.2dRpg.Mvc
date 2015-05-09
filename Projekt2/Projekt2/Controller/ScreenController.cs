using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Controller
{
    class ScreenController
    {
        private View.ScreenView m_screenView;
        private Model.GameModel m_gameModel;
        private Stopwatch m_watch = new Stopwatch();
        private SpriteBatch m_spriteBatch;
        private bool m_fullScreen;
        private bool m_doQuit;


        //Variabler för bild/texthantering
        public bool m_showStartScreen = true;
        public bool m_showClassSelectionScreen;
        private bool m_showPauseScreen;
        private bool m_showGameOverScreen;

        public ScreenController(Model.GameModel a_gameModel, SpriteBatch a_spriteBatch, View.AnimationSystem a_animationSystem, View.InputHandler a_inputHandler, View.SoundHandler a_soundHandler)
        {
            this.m_gameModel = a_gameModel;
            this.m_spriteBatch = a_spriteBatch;
            this.m_screenView = new View.ScreenView(m_spriteBatch, a_animationSystem, a_inputHandler, a_soundHandler);
        }

        public void LoadScreenContent(ContentManager a_content)
        {
            m_screenView.LoadContent(a_content);
        }

        internal void UpdateScreenSimulation(float a_elapsedTime)
        {
            if (!m_screenView.DidPressOptions)
                m_fullScreen = m_screenView.FullScreen;

            if (m_screenView.DidPressNewGame)
            {
                m_showStartScreen = false;
                m_showClassSelectionScreen = true;
            }

            //Anv valde klass
            if (m_screenView.DidChooseClass)
                m_showClassSelectionScreen = false;

            //Anv trycker escape
            if (m_screenView.PressedAndReleasedEsc() && !IsShowingExternalScreen() && !m_showPauseScreen)
                m_showPauseScreen = true;
            else if (m_screenView.PressedAndReleasedEsc())
                m_showPauseScreen = false;

            if (m_screenView.DidPressQuit)
                m_doQuit = true;

            if (m_gameModel.GameIsOver())
                m_showGameOverScreen = true;

        }

        internal void DrawScreens(float a_elapsedTime)
        {
            m_spriteBatch.Begin();

            if (m_showStartScreen)
                m_screenView.DrawStartScreen();

            if (m_showClassSelectionScreen)
                m_screenView.DrawClassSelectionScreen(a_elapsedTime);

            if (m_showPauseScreen)
            {
                m_screenView.DrawPauseScreen(a_elapsedTime);
            }

            if (m_showGameOverScreen)
            {
                m_screenView.DrawGameOverScreen(a_elapsedTime);
            }

            m_screenView.DrawMouse();

            m_spriteBatch.End();
        }

        public bool DoQuit
        {
            get { return m_doQuit; }
        }

        public bool DoExit
        {
            get { return m_screenView.DidPressExit; }
        }

        public bool FullScreen
        {
            get { return m_fullScreen; }
        }

        //Retunerar true om en text/bild skärm skall visas
        public bool IsShowingExternalScreen()
        {
            return m_showStartScreen ||
                   m_showClassSelectionScreen ||
                   m_showGameOverScreen;
        }

        public bool IsShowingPauseScreen
        {
            get { return m_showPauseScreen; }
            set { m_showPauseScreen = value; }
        }

    }
 }

