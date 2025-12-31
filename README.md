# Lost-Realms（迷失领域）
一款 Unity 开发的 2D 横版动作冒险游戏，包含完整的角色战斗、敌人 AI、关卡探索与资源管理系统，全程独立开发完成。

## 项目简介
- **游戏类型**：2D 横版动作冒险
- **开发周期**：2024.10 - 2025.12（个人独立开发）
- **核心玩法**：玩家控制主角在像素风关卡中探索，击败巡逻敌人，挑战多阶段 Boss，通过场景切换推进剧情，体验快节奏战斗与探索乐趣
- **技术定位**：聚焦 Unity 2D 游戏全流程开发，落地「输入适配、AI 行为树、资源异步加载、事件驱动架构」等核心技术

## 核心技术栈
### 1. 引擎与框架
- Unity 2022.3.xxf1（替换为你的实际 Unity 版本，如 2022.3.15f1）
- 架构设计：表现层-逻辑层-数据层 分层架构 + 事件驱动（ScriptableObject）
- 核心工具：InputSystem（输入适配）、Behavior Designer（行为树 AI）、Addressables（资源管理）、UGUI（界面开发）

### 2. 编程与工具链
- 主力语言：C#（游戏核心逻辑）
- 版本控制：Git（项目迭代管理）
- 性能分析：Unity Profiler（CPU/内存优化）
- 其他：Physics2D（碰撞检测）、Animator（动画系统）、对象池（UI/敌人复用）

### 3. 资源与素材
- 美术风格：像素风（角色/场景/道具）
- 素材说明：核心美术资源来自 Unity Asset Store（若你有独立绘制的素材，可改为「部分核心素材独立绘制，其余来自 Unity Asset Store」），已标注版权信息

## 核心亮点（技术实现+量化成果）
### 1. 角色控制系统
- 基于 InputSystem 实现「键盘+手柄」双端适配，攻击响应延迟 < 0.05s，操作流畅无卡顿
- 动画与逻辑同步：通过 Animator 状态机 + 关键帧事件绑定攻击判定，解决动作僵直与判定不同步问题
- 核心脚本：`Assets/Scripts/Player/PlayerController.cs`、`Assets/Scripts/Player/PlayerAttack.cs`

### 2. 敌人 AI 系统
- 普通敌人：采用「状态机模式」，抽象 `BaseEnemyAI.cs` 基类，实现巡逻→追击→攻击状态平滑切换
- Boss 战斗：通过 Behavior Designer 搭建行为树，实现「巡逻→仇恨触发→三段连击→AOE 技能→受伤硬直」多阶段逻辑
- 核心脚本：`Assets/Scripts/Enemy/BaseEnemyAI.cs`、`Assets/Scripts/Enemy/BossAI.cs`

### 3. 资源加载优化
- 基于 Addressables 实现资源分组异步加载（启动组+关卡组），场景切换时间从 2.5s 优化至 0.8s
- UI 元素（血条、伤害飘字）使用对象池复用，减少 GC 开销 90%，战斗场景帧率稳定 60FPS
- 核心脚本：`Assets/Scripts/Resource/AddressablesManager.cs`、`Assets/Scripts/Resource/SceneLoader.cs`

### 4. 事件通信系统
- 基于 ScriptableObject 实现事件驱动通信，解耦跨模块依赖（如场景加载、音效播放）
- 核心脚本：`Assets/Scripts/Event/SceneLoadEventSO.cs`、`Assets/Scripts/Event/PlayerAudioEventSO.cs`

## 演示链接（后续补充后直接替换）
- 游戏 Demo（itch.io）：[Lost-Realms - 2D 横版动作游戏](https://你的itch.io用户名.itch.io/lost-realms)（后续发布 itch.io 后替换为实际链接）
- 演示视频（B站）：[Lost-Realms 游戏开发演示](https://www.bilibili.com/video/BVxxxx/)（后续上传 B 站后替换为实际链接）
- 技术文档：`Docs/` 文件夹下（架构设计、性能优化、已知问题与优化计划）

## 运行说明（方便面试官快速启动项目）
### 1. 本地运行（Unity 编辑器）
1. 环境要求：Unity 2022.3 及以上版本
2. 操作步骤：
   - 克隆本仓库到本地：`git clone https://github.com/xiaotaoxu2-afk/Lost-Realms.git`
   - 打开 Unity Hub，点击「添加项目」，选择克隆后的项目文件夹
   - 等待项目导入完成后，打开 `Assets/Scenes/StartScene.unity`，点击运行按钮即可启动游戏

### 2. 独立包运行（PC 端）
1. 下载链接：[点击下载 Windows 独立包](https://你的itch.io用户名.itch.io/lost-realms)（后续发布 itch.io 后复用此链接）
2. 操作步骤：
   - 下载压缩包后解压，双击 `Lost-Realms.exe` 即可运行
   - 推荐分辨率：1920×1080（窗口化/全屏均可）

### 3. 操作按键
| 操作         | 键盘按键 | 手柄按键       |
|--------------|----------|----------------|
| 移动         | WASD     | 左摇杆         |
| 普通攻击     | J        | 手柄 X 键      |
| 跳跃         | K        | 手柄 A 键      |
| 技能 1       | L        | 手柄 Y 键      |
| 暂停/打开菜单 | Esc      | 手柄 Start 键  |

## 项目结构（清晰展示代码组织逻辑）
- Lost-Realms/
  - Assets/                // Unity 核心资源
    - Scripts/            // 核心 C# 脚本
      - Player/          // 玩家相关脚本（控制、攻击、动画）
      - Enemy/           // 敌人相关脚本（AI、攻击、状态）
      - Resource/        // 资源管理脚本（Addressables、场景加载）
      - Event/           // 事件系统脚本（ScriptableObject 事件）
      - UI/              // UI 相关脚本（面板管理、HUD 显示）
      - Tool/            // 工具类脚本（对象池、音频工具）
    - Prefabs/            // 预制体（玩家、敌人、UI 组件）
    - Scenes/             // 关卡场景（StartScene、Level1、Level2、BossScene）
    - Textures/           // 纹理资源（角色、场景、道具 Sprite）
    - Audio/              // 音频资源（背景音乐、音效）
  - Docs/                  // 技术文档
    - 架构设计.md         // 项目分层架构、模块划分、交互逻辑
    - 性能优化.md         // 优化背景、措施、量化成果
    - 已知问题与优化.md   // 现有小 bug、后续优化计划
  - .gitignore             // Unity 项目 Git 过滤规则
  - README.md              // 项目总览（本文档）

## 已知问题（透明化，体现复盘意识）
1. 切换窗口/全屏时，UI 部分元素轻微错位（推荐使用 1080P 分辨率运行）
2. 普通敌人动画偶尔出现 1-2 帧卡顿（后续计划通过对象池优化）
3. 详细问题与优化计划见：`Docs/已知问题与优化.md`

## 致谢（可选，提升文档温度）
- 美术资源感谢：Unity Asset Store 相关素材作者（若使用第三方素材，可补充具体素材名称，如「Pixel Adventure Assets 素材作者」）
- 技术学习参考：Unity 官方文档、B站 Unity 开发教程、《Unity 游戏开发设计模式》

---
**作者**：徐晓涛（替换为你的真实姓名）  
**求职意向**：Unity 游戏开发工程师（2D 方向，应届生）  
**联系邮箱**：你的邮箱@xxx.com（替换为你的真实邮箱，如 2963375501@qq.com）  
**GitHub**：https://github.com/xiaotaoxu2-afk
