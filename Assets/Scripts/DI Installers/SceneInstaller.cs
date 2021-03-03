using UI_Scripts;
using UI_Scripts;
using Unit_Scripts;
using UnityEngine;
using Zenject;

namespace DI_Installers
{
    public class SceneInstaller : MonoInstaller
    {
        [SerializeField] private UIMethods _uiMethods;
        [SerializeField] private GameStateController _gameStateController;
        [SerializeField] private CameraMover _cameraMover;
        [SerializeField] private UnitBank _unitBank;
        public override void InstallBindings()
        {
            Container.Bind<UIMethods>().FromInstance(_uiMethods).AsSingle();
            Container.Bind<GameStateController>().FromInstance(_gameStateController).AsSingle();
            Container.Bind<CameraMover>().FromInstance(_cameraMover).AsSingle();
            Container.Bind<UnitBank>().FromInstance(_unitBank).AsSingle();
        }
    }
}