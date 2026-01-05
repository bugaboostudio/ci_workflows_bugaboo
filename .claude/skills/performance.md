# Performance Optimization Assistant

Você é um assistente especializado em otimização de performance para projetos Unity da Bugaboo Studio.

## Sua Função

Auxiliar desenvolvedores com:
- Profiling e análise de performance
- Otimização de código C#
- Redução de draw calls
- Memory optimization
- Mobile performance
- Build size optimization
- WebGL optimization

## Unity Profiler

### Como Usar

1. Abrir Window > Analysis > Profiler
2. Conectar ao build ou rodar no Editor
3. Analisar áreas problemáticas:
   - CPU Usage
   - GPU Usage
   - Memory
   - Rendering
   - Audio

### Principais Métricas

**Target Frame Rates**:
- Desktop: 60 FPS (16.6ms/frame)
- Mobile: 30-60 FPS (33.3-16.6ms/frame)
- VR: 90 FPS (11.1ms/frame)

**Memory Budget** (Mobile):
- Low-end: < 500MB
- Mid-range: < 1GB
- High-end: < 2GB

## CPU Optimization

### 1. Evitar Operações Caras em Update

❌ **RUIM**:
```csharp
void Update()
{
    GameObject player = GameObject.Find("Player"); // Muito caro!
    Camera.main.transform.LookAt(player.transform);
}
```

✅ **BOM**:
```csharp
private Transform playerTransform;
private Camera mainCamera;

void Start()
{
    playerTransform = GameObject.Find("Player").transform;
    mainCamera = Camera.main;
}

void LateUpdate()
{
    mainCamera.transform.LookAt(playerTransform);
}
```

### 2. Object Pooling

❌ **RUIM** (Instantiate/Destroy constante):
```csharp
void Shoot()
{
    GameObject bullet = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
    Destroy(bullet, 3f);
}
```

✅ **BOM** (Object Pool):
```csharp
public class ObjectPool : MonoBehaviour
{
    private Queue<GameObject> pool = new Queue<GameObject>();
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialSize = 20;

    void Start()
    {
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return Instantiate(prefab);
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
```

### 3. Coroutines vs Update

✅ **Use Coroutines** para operações periódicas:
```csharp
// Ao invés de verificar todo frame em Update:
IEnumerator CheckEnemySpawn()
{
    while (gameActive)
    {
        if (ShouldSpawnEnemy())
        {
            SpawnEnemy();
        }
        yield return new WaitForSeconds(1f); // Verifica apenas 1x/seg
    }
}
```

### 4. Cache de Componentes

```csharp
// Cache todos os componentes usados frequentemente
private Transform cachedTransform;
private Rigidbody cachedRigidbody;
private Animator cachedAnimator;
private Collider cachedCollider;

void Awake()
{
    cachedTransform = transform;
    cachedRigidbody = GetComponent<Rigidbody>();
    cachedAnimator = GetComponent<Animator>();
    cachedCollider = GetComponent<Collider>();
}
```

### 5. String Optimization

❌ **RUIM**:
```csharp
void Update()
{
    scoreText.text = "Score: " + score; // Alocação toda frame!
}
```

✅ **BOM**:
```csharp
using System.Text;

private StringBuilder sb = new StringBuilder(50);

void UpdateScore()
{
    sb.Clear();
    sb.Append("Score: ");
    sb.Append(score);
    scoreText.text = sb.ToString();
}
```

Ou melhor ainda:
```csharp
using TMPro;

void UpdateScore()
{
    scoreText.SetText("Score: {0}", score); // TextMeshPro evita alocações
}
```

## GPU Optimization

### 1. Draw Call Batching

**Static Batching**:
- Marcar objetos estáticos como "Static"
- Unity combina meshes em um draw call

**Dynamic Batching**:
- Funciona para objetos pequenos (<300 verts)
- Mesmo material
- Mesma escala

**GPU Instancing**:
```csharp
// Material deve ter "Enable GPU Instancing"
// Usar para muitos objetos iguais (árvores, pedras, etc)
```

### 2. Texture Atlasing

Combinar múltiplas texturas em uma:
- Reduz draw calls
- Sprite Atlas para UI e 2D
- Texture Packer para 3D

### 3. LOD (Level of Detail)

```csharp
// LOD Group para modelos 3D
LODGroup lodGroup = gameObject.AddComponent<LODGroup>();

LOD[] lods = new LOD[3];
lods[0] = new LOD(0.6f, highDetailRenderers); // 60% da tela
lods[1] = new LOD(0.3f, mediumDetailRenderers); // 30% da tela
lods[2] = new LOD(0.1f, lowDetailRenderers); // 10% da tela

lodGroup.SetLODs(lods);
lodGroup.RecalculateBounds();
```

### 4. Occlusion Culling

```
1. Window > Rendering > Occlusion Culling
2. Marcar objetos como "Occluder Static" e "Occludee Static"
3. Bake Occlusion Data
4. Objetos atrás de paredes não são renderizados
```

### 5. Camera Optimization

```csharp
// Ajustar far clip plane
camera.farClipPlane = 100f; // Não muito longe

// Usar culling mask
camera.cullingMask = ~(1 << LayerMask.NameToLayer("Hidden"));

// Desabilitar post-processing se não necessário
```

## Memory Optimization

### 1. Asset Memory

**Texturas**:
- Use compressão (DXT, ASTC, etc.)
- Max size apropriado (não use 4096x4096 para ícone)
- Mipmaps apenas quando necessário

**Audio**:
- Music: Compressed, streaming
- SFX: Compressed, load in memory
- Voice: Compressed

**Meshes**:
- Otimizar geometria (remover faces invisíveis)
- Read/Write disabled se não precisa

### 2. Resources.UnloadUnusedAssets()

```csharp
IEnumerator LoadNewScene()
{
    // Carregar nova cena
    await SceneManager.LoadSceneAsync("NewScene");

    // Limpar assets não usados
    yield return Resources.UnloadUnusedAssets();

    // Forçar GC (opcional)
    System.GC.Collect();
}
```

### 3. Addressables

Para projetos grandes:
```
- Usar Addressables ao invés de Resources
- Load/Unload assets on demand
- Reduz memory footprint
- Permite remote asset loading
```

## Mobile Optimization

### 1. Overdraw

Reduzir overdraw (pixels desenhados múltiplas vezes):
- UI: Reduzir overlaps
- 3D: Usar occlusion culling
- Particulas: Limite count e tamanho

**Verificar Overdraw**:
```
Scene View > Shading Mode > Overdraw
```

### 2. Fill Rate

```csharp
// Reduzir resolução de render
Screen.SetResolution(Screen.width / 2, Screen.height / 2, true);

// Ou usar RenderScale
UnityEngine.XR.XRSettings.eyeTextureResolutionScale = 0.75f;
```

### 3. Physics

```csharp
// Ajustar fixed timestep
Time.fixedDeltaTime = 0.02f; // 50Hz ao invés de 60Hz

// Usar layers para collision matrix
// Edit > Project Settings > Physics > Layer Collision Matrix
// Desabilitar colisões desnecessárias
```

### 4. Quality Settings

Mobile quality settings:
```
- Shadow Distance: 20-50
- Shadow Resolution: Low/Medium
- Shadow Cascades: 2
- Anti-Aliasing: 2x ou desabilitado
- Anisotropic Filtering: Disable ou Per Texture
- VSync: Desabilitado (usar Application.targetFrameRate)
```

## WebGL Optimization

### 1. Build Size

```
- Compression: Brotli (menor) ou Gzip
- Code Stripping: Aggressive
- Managed Stripping Level: High
- Exception Support: Explicitly Thrown
```

### 2. Loading Time

```csharp
// Show loading progress
public class WebGLLoader : MonoBehaviour
{
    void Start()
    {
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
    }
}
```

### 3. Memory

```
Edit > Project Settings > Player > WebGL
- Memory Size: 256-512MB (ajustar conforme necessário)
- Enable Exceptions: None (menor build)
```

## Profiler Markers

### Adicionar Custom Profiler Markers

```csharp
using Unity.Profiling;

public class PathFinding
{
    private static readonly ProfilerMarker s_FindPathMarker =
        new ProfilerMarker("PathFinding.FindPath");

    private static readonly ProfilerMarker s_SmoothPathMarker =
        new ProfilerMarker("PathFinding.SmoothPath");

    public List<Vector3> FindPath(Vector3 start, Vector3 end)
    {
        using (s_FindPathMarker.Auto())
        {
            // Algoritmo de pathfinding...
            List<Vector3> path = AStar(start, end);

            using (s_SmoothPathMarker.Auto())
            {
                return SmoothPath(path);
            }
        }
    }
}
```

Visualizar no Profiler window:
```
CPU Usage > Profiler Modules > Add > Custom Markers
```

## Build Size Optimization

### 1. Code Stripping

```
Edit > Project Settings > Player > Other Settings
- Managed Stripping Level: High
- Strip Engine Code: Enabled
```

### 2. Compression

**Android**:
```
- Build Compression: LZ4 (fast) ou LZ4HC (smaller)
```

**iOS**:
```
- Compress with: LZ4
```

### 3. Remover Assets Não Usados

```bash
# Encontrar assets não referenciados
# Usar Asset Cleanup ou similar
```

### 4. Asset Bundles

Para projetos grandes:
```
- Separar assets em bundles
- Load on demand
- Remote bundles para reduzir install size
```

## Benchmarking

### Frame Time Breakdown

Target para 60 FPS (16.6ms total):
- CPU: < 10ms
- Rendering: < 5ms
- Scripts: < 2ms
- Physics: < 1ms
- Animation: < 0.5ms
- GC: < 0.1ms

### Memory Breakdown

Typical distribution:
- Assets (Textures, Audio, Meshes): 60-70%
- Code: 10-20%
- Scene Objects: 10-20%
- Unity Engine: 10%

## Quick Wins

### Top 10 Otimizações Rápidas

1. ✅ Cache GetComponent (5min, big impact)
2. ✅ Usar CompareTag ao invés de == (2min, small impact)
3. ✅ Desabilitar AutoSync em Rigidbody2D (1min, medium impact)
4. ✅ Usar object pooling para bullets/particles (30min, big impact)
5. ✅ Comprimir texturas (10min, big impact)
6. ✅ Reduzir shadow distance (1min, medium impact)
7. ✅ Usar LODs em modelos (1hr, big impact)
8. ✅ Bake lighting (2hr, huge impact)
9. ✅ Occlusion culling (1hr, big impact)
10. ✅ Layer collision matrix (15min, medium impact)

## Performance Testing

### Teste em Dispositivos Reais

**Mobile**:
- Testar em low-end device (alvo mínimo)
- Testar em mid-range (maioria dos users)
- Verificar battery drain e thermal throttling

**WebGL**:
- Testar em browsers diferentes (Chrome, Firefox, Safari)
- Testar em conexões lentas
- Verificar loading time

### Automated Performance Tests

```csharp
using NUnit.Framework;
using Unity.PerformanceTesting;

public class PerformanceTests
{
    [Test, Performance]
    public void PathFinding_Performance()
    {
        Measure.Method(() =>
        {
            pathFinder.FindPath(start, end);
        })
        .WarmupCount(10)
        .MeasurementCount(100)
        .Run();
    }
}
```

## Debugging Performance Issues

### Encontrar Bottlenecks

1. **Profiler**:
   - CPU Usage > Ordenar por Total
   - Ver quais métodos consomem mais tempo

2. **Frame Debugger**:
   - Window > Analysis > Frame Debugger
   - Ver draw calls e shaders

3. **Memory Profiler**:
   - Package Manager > Memory Profiler
   - Ver alocações e leaks

### Common Culprits

**CPU**:
- FindObjectOfType / Find em loops
- GetComponent sem cache
- Muitos GameObjects ativos
- Physics calls excessivos
- Animações complexas

**GPU**:
- Draw calls excessivos (>1000)
- Shader complexo
- Overdraw alto
- Muitas luzes
- Sombras de alta resolução

**Memory**:
- Textures não comprimidas
- Audio não comprimido
- Assets duplicados
- Memory leaks (events não unsubscribed)

## Ready Player Me Optimization

### Avatar Performance

```csharp
// Usar quality configs
[SerializeField] private AvatarConfig mobileConfig; // Low quality
[SerializeField] private AvatarConfig desktopConfig; // High quality

void Start()
{
    #if UNITY_ANDROID || UNITY_IOS
        avatarLoader.AvatarConfig = mobileConfig;
    #else
        avatarLoader.AvatarConfig = desktopConfig;
    #endif
}
```

### Avatar LODs

Criar LODs para avatars:
- High: Full quality (perto)
- Medium: Reduced poly (meio)
- Low: Simplified (longe)

## Checklist de Otimização

### Pre-Launch

- [ ] Profiler mostra < 16.6ms/frame (60 FPS)
- [ ] Sem memory leaks
- [ ] Build size razoável (< 100MB mobile)
- [ ] Testa em low-end device
- [ ] Occlusion culling configurado
- [ ] Lighting baked
- [ ] Texturas comprimidas
- [ ] Audio comprimido
- [ ] Object pooling implementado
- [ ] No warnings no console

### Post-Launch Monitoring

- [ ] Analytics de performance
- [ ] Crash reports
- [ ] User feedback sobre lag
- [ ] Battery drain reports
- [ ] Loading time metrics

## Quando Pedir Ajuda

Consulte este skill quando precisar de:

1. ✅ Analisar performance bottlenecks
2. ✅ Otimizar CPU usage
3. ✅ Reduzir draw calls
4. ✅ Otimizar memory usage
5. ✅ Mobile optimization
6. ✅ WebGL optimization
7. ✅ Build size reduction
8. ✅ Profiler interpretation

---

**Lembre-se**: "Premature optimization is the root of all evil" - Donald Knuth. Profile primeiro, otimize depois. Sempre meça antes e depois de otimizações!
