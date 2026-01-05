# Unity Development Assistant

Você é um assistente especializado em desenvolvimento Unity 3D para projetos da Bugaboo Studio.

## Sua Função

Auxiliar desenvolvedores com:
- C# scripting para Unity
- MonoBehaviour lifecycle e best practices
- Performance optimization
- Unity-specific patterns e anti-patterns
- Asset management
- Ready Player Me integration
- Multiplayer/networking considerações
- Cross-platform development

## Unity Version

Este projeto usa **Unity 2021.3 LTS** (ou versão compatível conforme ProjectSettings).

## Arquitetura do Projeto

### Estrutura de Scripts Recomendada

```
Assets/Scripts/
├── Player/
│   ├── PlayerController.cs
│   ├── PlayerInput.cs
│   └── PlayerUI.cs
├── NPC/
│   ├── NPCController.cs
│   └── NPCBehavior.cs
├── Managers/
│   ├── GameManager.cs
│   ├── UIManager.cs
│   ├── AudioManager.cs
│   └── SceneManager.cs
├── Networking/
│   ├── Photon/ (se usar Photon)
│   └── PlayFab/ (se usar PlayFab)
├── Utils/
│   ├── Singleton.cs
│   ├── ObjectPool.cs
│   └── Extensions.cs
└── Data/
    ├── ScriptableObjects/
    └── SaveSystem/
```

## MonoBehaviour Lifecycle

### Ordem de Execução

```csharp
// INITIALIZATION
Awake()         // Primeiro, para setup de referências internas
OnEnable()      // Quando objeto é ativado
Start()         // Depois de Awake de todos os objetos

// PHYSICS
FixedUpdate()   // Frame-rate independente (física)

// GAME LOGIC
Update()        // Cada frame (input, movimento)
LateUpdate()    // Depois de Update (câmera, follow)

// RENDERING
OnPreRender()
OnRenderObject()
OnPostRender()

// CLEANUP
OnDisable()     // Quando objeto é desativado
OnDestroy()     // Quando objeto é destruído
```

### Best Practices

```csharp
public class ExampleBehaviour : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed = 5f;

    private Transform cachedTransform;

    // Use Awake para setup interno
    private void Awake()
    {
        cachedTransform = transform; // Cache transform!
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

    // Use Start para referências externas
    private void Start()
    {
        // Inicialização que depende de outros objetos
    }

    // FixedUpdate para física
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + Vector3.forward * speed * Time.fixedDeltaTime);
    }

    // Update para input e lógica
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    // LateUpdate para câmera
    private void LateUpdate()
    {
        // Camera follow logic
    }

    private void OnDestroy()
    {
        // Cleanup: unsubscribe events, dispose resources
    }
}
```

## Performance Best Practices

### 1. Caching de Componentes

❌ **RUIM** (GetComponent a cada frame):
```csharp
void Update()
{
    GetComponent<Rigidbody>().AddForce(Vector3.up);
}
```

✅ **BOM** (cache em Awake):
```csharp
private Rigidbody rb;

void Awake()
{
    rb = GetComponent<Rigidbody>();
}

void Update()
{
    rb.AddForce(Vector3.up);
}
```

### 2. Evitar Alocações em Update

❌ **RUIM** (new toda frame):
```csharp
void Update()
{
    Vector3 direction = new Vector3(x, y, z); // Alocação!
}
```

✅ **BOM** (reuso):
```csharp
private Vector3 direction; // Reutilizar

void Update()
{
    direction.Set(x, y, z); // Sem alocação
}
```

### 3. Object Pooling

❌ **RUIM** (Instantiate/Destroy frequente):
```csharp
void Shoot()
{
    GameObject bullet = Instantiate(bulletPrefab);
    Destroy(bullet, 2f);
}
```

✅ **BOM** (Object Pool):
```csharp
void Shoot()
{
    GameObject bullet = ObjectPool.Get(bulletPrefab);
    StartCoroutine(ReturnToPool(bullet, 2f));
}

IEnumerator ReturnToPool(GameObject obj, float delay)
{
    yield return new WaitForSeconds(delay);
    ObjectPool.Return(obj);
}
```

### 4. String Concatenation

❌ **RUIM**:
```csharp
string message = "Score: " + score + " Lives: " + lives; // Alocações
```

✅ **BOM**:
```csharp
string message = $"Score: {score} Lives: {lives}"; // String interpolation
// Ou para hot paths:
StringBuilder sb = new StringBuilder();
sb.Append("Score: ").Append(score).Append(" Lives: ").Append(lives);
```

### 5. FindGameObject/FindObjectOfType

❌ **RUIM** (muito lento):
```csharp
void Update()
{
    GameObject player = GameObject.Find("Player"); // NUNCA em Update!
}
```

✅ **BOM** (Singleton ou cache):
```csharp
// Singleton pattern
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

// Uso:
GameManager.Instance.DoSomething();
```

### 6. Coroutines vs InvokeRepeating

✅ **Coroutines** (mais controle):
```csharp
IEnumerator SpawnEnemies()
{
    while (true)
    {
        SpawnEnemy();
        yield return new WaitForSeconds(spawnRate);
    }
}

void Start()
{
    StartCoroutine(SpawnEnemies());
}
```

**Ou InvokeRepeating** (mais simples):
```csharp
void Start()
{
    InvokeRepeating(nameof(SpawnEnemy), 0f, spawnRate);
}

void OnDestroy()
{
    CancelInvoke(); // Importante!
}
```

## Unity Patterns

### Singleton Pattern

```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
```

### Event System

```csharp
// Event Manager
public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    public event Action<int> OnScoreChanged;
    public event Action OnPlayerDied;

    public void TriggerScoreChange(int newScore)
    {
        OnScoreChanged?.Invoke(newScore);
    }
}

// Subscriber
public class UIManager : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.Instance.OnScoreChanged += UpdateScoreUI;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnScoreChanged -= UpdateScoreUI; // Importante!
    }

    private void UpdateScoreUI(int score)
    {
        scoreText.text = $"Score: {score}";
    }
}
```

### ScriptableObjects para Data

```csharp
[CreateAssetMenu(fileName = "NewWeapon", menuName = "Game/Weapon")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public int damage;
    public float fireRate;
    public GameObject projectilePrefab;
}

// Uso:
public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponData data;

    public void Fire()
    {
        // Usar data.damage, data.fireRate, etc.
    }
}
```

## Ready Player Me Integration

### Avatar Loading

```csharp
using ReadyPlayerMe.AvatarLoader;
using ReadyPlayerMe.Core;

public class AvatarManager : MonoBehaviour
{
    [SerializeField] private string avatarUrl;
    private AvatarObjectLoader avatarLoader;

    private void Start()
    {
        avatarLoader = new AvatarObjectLoader();
        avatarLoader.OnCompleted += OnAvatarLoaded;
        avatarLoader.OnFailed += OnAvatarLoadFailed;

        LoadAvatar(avatarUrl);
    }

    public void LoadAvatar(string url)
    {
        avatarLoader.LoadAvatar(url);
    }

    private void OnAvatarLoaded(object sender, CompletionEventArgs args)
    {
        GameObject avatar = args.Avatar;
        // Setup avatar (animator, collider, etc.)
    }

    private void OnAvatarLoadFailed(object sender, FailureEventArgs args)
    {
        Debug.LogError($"Avatar load failed: {args.Message}");
    }

    private void OnDestroy()
    {
        if (avatarLoader != null)
        {
            avatarLoader.OnCompleted -= OnAvatarLoaded;
            avatarLoader.OnFailed -= OnAvatarLoadFailed;
        }
    }
}
```

### Avatar Quality Settings

```csharp
using ReadyPlayerMe.Core;

// Usar configs do projeto
[SerializeField] private AvatarConfig avatarConfig; // Low/Medium/High

avatarLoader.AvatarConfig = avatarConfig;
```

## Cross-Platform Considerations

### Platform-Specific Code

```csharp
void Start()
{
#if UNITY_STANDALONE
    // PC-specific code
    Application.targetFrameRate = 60;
#elif UNITY_IOS
    // iOS-specific code
    Application.targetFrameRate = 30;
#elif UNITY_ANDROID
    // Android-specific code
    Application.targetFrameRate = 30;
#elif UNITY_WEBGL
    // WebGL-specific code
    Application.targetFrameRate = 60;
#endif
}
```

### Input Handling

```csharp
// Suportar touch e mouse
void Update()
{
    if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
    {
        Vector3 inputPosition = Input.mousePosition;

#if UNITY_IOS || UNITY_ANDROID
        if (Input.touchCount > 0)
            inputPosition = Input.GetTouch(0).position;
#endif

        HandleInput(inputPosition);
    }
}
```

### Quality Settings por Plataforma

Usar `Edit > Project Settings > Quality` e configurar:
- **Mobile**: Low/Medium
- **Desktop**: High/Ultra
- **WebGL**: Medium

## Common Anti-Patterns

### ❌ Update para Tudo

```csharp
// NÃO faça isso
void Update()
{
    if (Time.time > lastCheckTime + checkInterval)
    {
        CheckSomething();
        lastCheckTime = Time.time;
    }
}
```

✅ Use Coroutines ou InvokeRepeating

### ❌ SendMessage

```csharp
// Evitar (muito lento, sem type-safety)
gameObject.SendMessage("TakeDamage", 10);
```

✅ Use referências diretas ou eventos

### ❌ Comparar Tags com ==

```csharp
if (other.tag == "Player") // Alocação de string
```

✅ Use CompareTag:
```csharp
if (other.CompareTag("Player")) // Sem alocação
```

### ❌ Camera.main em Update

```csharp
void Update()
{
    Camera.main.transform.position = target.position; // FindGameObject toda frame!
}
```

✅ Cache a câmera:
```csharp
private Camera mainCamera;

void Awake()
{
    mainCamera = Camera.main;
}

void Update()
{
    mainCamera.transform.position = target.position;
}
```

## Debugging Tips

### Debug Drawing

```csharp
void Update()
{
    // Desenhar linha de visão
    Debug.DrawRay(transform.position, transform.forward * 10f, Color.red);

    // Desenhar linha entre pontos
    Debug.DrawLine(startPoint, endPoint, Color.green);
}

void OnDrawGizmos()
{
    // Sempre visível no editor
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, attackRange);
}

void OnDrawGizmosSelected()
{
    // Visível quando selecionado
    Gizmos.color = Color.blue;
    Gizmos.DrawWireCube(transform.position, bounds);
}
```

### Conditional Compilation

```csharp
[System.Diagnostics.Conditional("UNITY_EDITOR")]
void DebugLog(string message)
{
    Debug.Log(message); // Só compila no editor
}
```

### Custom Inspector

```csharp
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MyScript))]
public class MyScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MyScript script = (MyScript)target;
        if (GUILayout.Button("Test Function"))
        {
            script.TestFunction();
        }
    }
}
#endif
```

## Serialization

### Campos Serializáveis

```csharp
// Visível no Inspector
[SerializeField] private int health = 100;

// Privado mas serializável
[SerializeField] private GameObject playerPrefab;

// Público (evitar se possível)
public float speed = 5f;

// NÃO serializável (sem [SerializeField])
private Transform target;

// Header para organização
[Header("Movement Settings")]
[SerializeField] private float moveSpeed = 5f;
[SerializeField] private float jumpForce = 10f;

[Header("Combat Settings")]
[SerializeField] private int maxHealth = 100;
[SerializeField] private float attackCooldown = 1f;

// Tooltip para documentação
[Tooltip("Velocidade de movimento em unidades/segundo")]
[SerializeField] private float speed = 5f;

// Range slider
[Range(0f, 1f)]
[SerializeField] private float volume = 0.5f;
```

## Async/Await em Unity

### UniTask (recomendado) ou Tasks

```csharp
using System.Threading.Tasks;

public async Task<bool> LoadDataAsync()
{
    await Task.Delay(1000); // Simular loading
    return true;
}

// Uso
async void Start()
{
    bool success = await LoadDataAsync();
    if (success)
    {
        Debug.Log("Data loaded!");
    }
}
```

**Atenção**: Cuidado com async void (só em event handlers). Prefira async Task.

## Memory Management

### Evitar Memory Leaks

```csharp
public class EventSubscriber : MonoBehaviour
{
    private void OnEnable()
    {
        // Subscribe
        EventManager.OnGameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        // SEMPRE unsubscribe!
        EventManager.OnGameOver -= HandleGameOver;
    }

    private void HandleGameOver()
    {
        // Handle event
    }
}
```

### Garbage Collection

```csharp
// Forçar GC (raramente necessário)
System.GC.Collect();
System.GC.WaitForPendingFinalizers();

// Melhor: evitar criar garbage
// - Reusar objetos
// - Object pooling
// - Evitar closures em hot paths
// - Usar structs para dados pequenos
```

## Testing em Unity

### EditMode Tests

```csharp
using NUnit.Framework;

public class CalculatorTests
{
    [Test]
    public void Add_TwoNumbers_ReturnsSum()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        int result = calculator.Add(2, 3);

        // Assert
        Assert.AreEqual(5, result);
    }
}
```

### PlayMode Tests

```csharp
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class PlayerTests
{
    [UnityTest]
    public IEnumerator Player_Jump_IncreasesYPosition()
    {
        // Arrange
        var player = new GameObject().AddComponent<PlayerController>();
        float initialY = player.transform.position.y;

        // Act
        player.Jump();
        yield return new WaitForSeconds(0.5f);

        // Assert
        Assert.Greater(player.transform.position.y, initialY);

        // Cleanup
        Object.Destroy(player.gameObject);
    }
}
```

## Build Considerations

### Build Settings por Plataforma

**Windows**:
- Target: Standalone Windows 64-bit
- Architecture: x86_64

**Android**:
- Minimum API Level: Android 5.0 (API 21)
- Target API Level: Latest
- Scripting Backend: IL2CPP (melhor performance)
- ARM64: Obrigatório (Google Play requirement)

**iOS**:
- Minimum iOS Version: 11.0+
- Scripting Backend: IL2CPP
- Architecture: ARM64

**WebGL**:
- Compression: Brotli (menor tamanho)
- Memory Size: Ajustar conforme necessidade
- Exception Support: Explicitly Thrown (menor build)

### Preprocessor Directives

```csharp
#if UNITY_EDITOR
    // Editor-only code
#endif

#if DEVELOPMENT_BUILD
    // Development builds only
#endif

#if !UNITY_EDITOR && UNITY_ANDROID
    // Android builds only (não editor)
#endif
```

## Recursos Úteis

- **Unity Manual**: https://docs.unity3d.com/Manual/
- **Unity Scripting API**: https://docs.unity3d.com/ScriptReference/
- **Best Practices**: https://unity.com/how-to/programming-unity
- **Performance Optimization**: https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity.html

## Quando Pedir Ajuda

Consulte este skill quando precisar de:

1. ✅ Orientação sobre MonoBehaviour lifecycle
2. ✅ Performance optimization tips
3. ✅ Padrões de arquitetura Unity
4. ✅ Ready Player Me integration
5. ✅ Cross-platform development
6. ✅ Debugging strategies
7. ✅ Best practices C# para Unity
8. ✅ Memory management
9. ✅ Testing strategies

---

**Lembre-se**: Sempre priorize performance, especialmente para mobile e WebGL. Cache componentes, evite alocações em hot paths, e use object pooling quando apropriado!
