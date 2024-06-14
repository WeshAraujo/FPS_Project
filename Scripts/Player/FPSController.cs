using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Esse script usa cinemachine e InputSystem
namespace StarterAssets  //Esse namespace é só para testar por enquanto, estou usando o starter assets da unity
{
    public class FPSController : MonoBehaviour
    {
        public enum MoveType { Real, Arcade}
        public MoveType moveType = MoveType.Arcade;
        [Header("Atributos de Movimentação")]
        [Tooltip("Velocidade de movimento normal")]
        public float moveSpeed;
        [Tooltip("Velocidade de movimento ao correr")]
        public float sprintSpeed;
        [Tooltip("Velocidade de movimento quando agachado")]
        public float crouchSpeed;
        [Tooltip("Velocidade de movimento quando sliding")]
        public float slideSpeed;
        [Tooltip("Velocidade de movimento quando sliding")]
        public float slideTime;
        [Tooltip("Velocidade de rodar o personagem / olhar para os lados/ serve como uma sensibilidade do mouse/analogico")]
        public float rotationSpeed;
        [Tooltip("Serve para garantir um efeito de aceleração e desaceleração")]
        public float speedChangeRate;
        [Tooltip("Altura do Personagem quando agachado")]
        public float crouchHeight;
        [Tooltip("Altura do Personagem quando em pé")]
        private float standingHeight;
        [Tooltip("Velocidade de rodar o personagem / olhar para os lados/ serve como uma sensibilidade do mouse/analogico")]
        public float sensibilidadeMouse;
        public float sensibilidadeAnalogico;

        [Space(10)]
        [Tooltip("O quão alto o player pode pular")]
        public float JumpHeight = 1.2f;
        [Tooltip("O valor da gravidade que o player recebe")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Tempo necessário para passar antes de poder saltar novamente. Defina como 0f para pular instantaneamente novamente")]
        public float JumpTimeout = 0.1f;
        [Tooltip("Tempo necessário para passar antes de entrar no estado de queda. Útil para descer escadas")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("Se o personagem está no chão ou não. Não faz parte do CharacterController integrado na verificação")]
        public bool Grounded = true;
        [Tooltip("Útil para terrenos acidentados")]
        public float GroundedOffset = -0.14f;
        [Tooltip("O raio da verificação no chao. Deve corresponder ao raio do CharacterController")]
        public float GroundedRadius = 0.5f;
        [Tooltip("Quais camadas o personagem usa como seu chão")]
        public LayerMask GroundLayers;

        [Header("Atributos da camera")]
        [Tooltip("O alvo a seguir definido na Câmera Virtual")]
        public GameObject CinemachineCameraTarget;
        public GameObject CameraWeaponTarget;
        [Tooltip("Quanto podera mover a camera para cima, em angulos")]
        public float TopClamp = 90.0f;
        [Tooltip("Quanto podera mover a camera para baixo, em angulos")]
        public float BottomClamp = -90.0f;


        private bool sliding;
        private bool canSlide = true;
        // cinemachine
        private float _cinemachineTargetPitch;

        // player move
        private float _speed;
        private float _verticalVelocity;
        //player camera
        private float _rotationVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime. Variaveis para controlar um tempo mínimo sem pular e sem aplicar física de queda, ajuda para escadarias.
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        //componentesNecessários
        private PlayerInput _playerInput; //Esse é o componente do Input System
        private CharacterController _controller; // é o componente CharacterController
        private StarterAssetsInputs _input; //É o script que tem todos os métodos de Input;
        private GameObject _mainCamera;
        public Animator anim;
        public ChangeWeapon changeWeapon;

        public CinemachineVirtualCamera virtualCamera;
        private CinemachineBasicMultiChannelPerlin noise;

        public float _threshold = 0.01f; //é uma contante que define a zona morta do movimento do analógico.

        //serve para saber se o mouse está sendo usado para movimentar a camera
        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }
        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            _playerInput = GetComponent<PlayerInput>();
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            standingHeight = _controller.height;
            if(moveType == MoveType.Real)
            {
                SetRealValue();
            }
            noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        }

        // Update is called once per frame
        void Update()
        {
            Move();
            GroundedCheck();
            JumpAndGravity();
            Crouch();
            Slide();
        }
        private void LateUpdate()
        {
            CameraRotation();
        }
        private void Move()
        {
            //verifica se está correndo, se estiver
            if(_input.sprint && _input.move.magnitude != 0 && !_input.crouching)
            {
                changeWeapon.anim.SetBool("Run", true);
                //verfica se a movimentacao esta real, se tiver, aplica um noise na camera
                if(moveType == MoveType.Real)
                {
                    noise.m_AmplitudeGain = 4.0f; 
                    noise.m_FrequencyGain = 6.0f; 
                }
            }
            else
            {
                changeWeapon.anim.SetBool("Run", false);
                noise.m_AmplitudeGain = 0f;
                noise.m_FrequencyGain = 0f;
            }
            //verifica se o booleano que represeta a corrida se está verdadeiro, se for, recebe a velocidade de sprint, caso nao, recebe a velocidade Move
            //float velocidadeAlvo = _input.sprint ? sprintSpeed : moveSpeed;

            //Verifica os Inputs para saber quais velocidades devem ser aplicadas no player, de acordo com agachado, corrando ou andando (nao correndo), sliding
            float velocidadeAlvo = crouchSpeed;
            if (_input.sprint && !_input.crouching && !sliding && !_input.aiming)
            {
                velocidadeAlvo = sprintSpeed;
            }
            else if(!_input.sprint && !_input.crouching && !sliding && !_input.aiming)
            {
                velocidadeAlvo = moveSpeed;
            }
            else if(_input.crouching && !_input.sprint && !sliding || _input.aiming)
            {
                velocidadeAlvo = crouchSpeed;
            }
            else if(sliding)
            {
                velocidadeAlvo = slideSpeed;
            }

            // verifica se o input for zero. (se nao tem movimento, logo, a velocidade tambem vai para zero
            if (_input.move == Vector2.zero) 
            {
                velocidadeAlvo = 0f;
            }
            //calvula a velocidade horizontal
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            //nao sei para que serve
            float speedOffset = 0.1f;
            //verifica se está usando analogico ou digital para realizar o movimento de aceleração
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // serve para acelerar ou desacelerar o movimento
            if (currentHorizontalSpeed < velocidadeAlvo - speedOffset || currentHorizontalSpeed > velocidadeAlvo + speedOffset)
            {
                //Cria uma curva, fazendo um movimento mais organico.
                _speed = Mathf.Lerp(currentHorizontalSpeed, velocidadeAlvo * inputMagnitude, Time.deltaTime * speedChangeRate);

               //Arrendonda para 3 casas decimais
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = velocidadeAlvo;
            }
            // Traz um vetor normalizado
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            //serve para trazer a movimentacao do player de acordo com a visão da camera, não de acordo com um vector3 padrao
            if (_input.move != Vector2.zero)
            {
                // movimenta
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            }

            // movimenta o jogador de fato, trazendo a direcao dos input, mais a direçao da gravidade
            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }
        private void CameraRotation()
        {
            // Define uma zona morta para a entrada da movimentacao da camera
            if (_input.look.sqrMagnitude >= _threshold)
            {
                //verfica qual input esta sendo usado (keyboard ou gamepad)
                string currentScheme = _playerInput.currentControlScheme;
                if (currentScheme == "KeyboardMouse" && !_input.aiming)
                {
                    //recebe os valores para rotacionar
                    _cinemachineTargetPitch += _input.look.y * sensibilidadeMouse;
                    _rotationVelocity = _input.look.x * sensibilidadeMouse;
                }
                else if (currentScheme == "Gamepad" && !_input.aiming)
                {
                    _cinemachineTargetPitch += _input.look.y * sensibilidadeAnalogico * Time.deltaTime;
                    _rotationVelocity = _input.look.x * sensibilidadeAnalogico * Time.deltaTime;
                }
                else if (currentScheme == "KeyboardMouse" && _input.aiming)
                {
                    //recebe os valores para rotacionar
                    _cinemachineTargetPitch += _input.look.y * sensibilidadeMouse/2;
                    _rotationVelocity = _input.look.x * sensibilidadeMouse/2;
                }
                else if (currentScheme == "Gamepad" && _input.aiming)
                {
                    _cinemachineTargetPitch += _input.look.y * sensibilidadeAnalogico/2 * Time.deltaTime;
                    _rotationVelocity = _input.look.x * sensibilidadeAnalogico/2 * Time.deltaTime;
                }

                // fixa a rotaçao para não passar dos angulos definidos pra cima e para baixo
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, TopClamp, BottomClamp);

                // Atualiza a camera cima baixo 
                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
                CameraWeaponTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

                // rotaciona o player(esse gameObject) para seguir a rotacao da camera esquerda direita
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }
        // Calculo da rotaçao para não passar dos angulos definidos pra cima e para baixo
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
        private void GroundedCheck()
        {
            // Define a posicão da Esfera que será criada
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);

            //cria uma esfera que checa colisão dentro do seu raio e se fizer parte das camadas selecionadas, se tiver colisao, será true, caso nao, sera false
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }
        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reseta o tempo de queda
                _fallTimeoutDelta = FallTimeout;

                // impedir que a velocidade de queda aumente infinitamente quando estiver no chao
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // verifica se a booleana de pulo é verdadeira e se o tempo para pular novamente ja passou
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // applica a força de pulo, essa função serve para que a variavel jumpHeight possa ser definida com valores próximos aos reais
                    //exemplo: se a variavel for 1f, o player pula alturas de 1m. se for 2f, o player pula 2m de altura
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }

                // contador do tempo de pular novamente
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reseta o tempo para pular novamente
                _jumpTimeoutDelta = JumpTimeout;

                // contador do tempo de queda
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }

                // se nao estiver no chao, impede de pular novamente.
                _input.jump = false;
            }

            // aplica gravidade ao longo do tempo se não ultrapassar a velocidade do terminal... nao sei o que é;  (multiplique pelo tempo delta duas vezes para acelerar linearmente ao longo do tempo)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }
        private void Crouch()
        {
            //verifica se Input de agachar foi chamado e muda a altura do Player para simular que esta agachado
            if(_input.crouching || sliding)
            {
                _controller.height = crouchHeight;
            }
            else
            {
                _controller.height = Mathf.Lerp(_controller.height, standingHeight, Time.deltaTime * 6);
            }
        }
        private void Slide()
        {
            //Verifica se Input de Slide está pressionado e se o tipo de movimento é arcade
            if(_input.sprint && _input.crouching)
            {
                if(canSlide && moveType == MoveType.Arcade)
                {
                    StartCoroutine(TimeSlide());
                }
            }
        }
        private IEnumerator TimeSlide()
        {
            changeWeapon.anim.CrossFadeInFixedTime("Slide", 0.1f);
            sliding = true;
            canSlide = false;
            yield return new WaitForSeconds(slideTime);
            sliding = false;
            yield return new WaitForSeconds(2f);
            canSlide = true;
        }
        private void SetRealValue()
        {
            //Caso o player escolha movimentacao Real, esses parametros serao adaptados
            moveSpeed = 3;
            sprintSpeed = 6;
            crouchSpeed = 1.2f;
        }

        //Metodo de Aplicar Recuo de forma diferente do escolhido. Só chamar esse metodo diretamente do metodo de Tiro da Arma.
        public void ApplyRecoil(float horizontal, float vertical)
        {
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
            _cinemachineTargetPitch += _input.look.y * vertical * rotationSpeed * deltaTimeMultiplier;
            _rotationVelocity = _input.look.x * horizontal * rotationSpeed * deltaTimeMultiplier;
        }
        //Ajusta a sensibilidade do mouse. É chamado diretamente da Ui do jogo
        public void ChangeMouseSensibility( float value)
        {
            sensibilidadeMouse = value;
        }
        //Ajusta a sensibilidade do Analogico. É chamado diretamente da Ui do jogo
        public void ChangeGamepadSensibility(float value)
        {
            sensibilidadeAnalogico = value;
        }
    }
}
