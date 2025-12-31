# Lost-Realms
一款 Unity 开发的 2D 横版动作冒险游戏，包含完整的角色战斗、敌人 AI、关卡探索与资源管理系统，全程独立开发完成。

## 项目简介
- **游戏类型**：2D 横版动作冒险
- **开发周期**：2025.9 - 2025.12
- **核心玩法**：玩家控制主角在像素风关卡中探索，击败巡逻敌人，挑战多阶段 Boss，体验快节奏战斗与探索乐趣
- **技术定位**：聚焦 Unity 2D 游戏全流程开发，落地「输入适配、AI 行为树、资源异步加载、事件驱动架构」等核心技术

## 核心技术栈
### 1. 引擎与框架
- Unity 2022.3.62f1
- 架构设计：表现层-逻辑层-数据层 分层架构 + 事件驱动（ScriptableObject）
- 核心工具：InputSystem（输入适配）、Behavior Designer（行为树 AI）、Addressables（资源管理）、UGUI（界面开发）

### 2. 编程与工具链
- 主力语言：C#（游戏核心逻辑）
- 版本控制：Git（项目迭代管理）
- 性能分析：Unity Profiler（CPU/内存优化）
- 其他：Physics2D（碰撞检测）、Animator（动画系统）、对象池（UI/敌人复用）

## 核心亮点
### 1. 角色控制系统
- 动画与逻辑同步：通过 Animator 状态机 + 关键帧事件绑定攻击判定，解决动作僵直与判定不同步问题
- 核心脚本：`Assets/Scripts/Player/PlayerController.cs`、`Assets/Scripts/Player/PlayerAttack.cs`

### 2. 敌人 AI 系统
- 普通敌人：采用「状态机模式」，抽象 `BaseEnemyAI.cs` 基类，实现巡逻→追击→攻击状态平滑切换
- Boss 战斗：通过 Behavior Designer 搭建行为树，实现「仇恨触发→三段连击→AOE 技能→受伤」多阶段逻辑
- 核心脚本：`Assets/Scripts/Enemy/BossController.cs`、`Assets/Scripts/Enemy/BossAI.cs`

### 3. 资源加载优化
- 基于 Addressables 实现资源分组异步加载，场景切换时间优化减少
- 核心脚本：`Assets/Scripts/Resource/AddressablesManager.cs`、`Assets/Scripts/Resource/SceneLoader.cs`

### 4. 事件通信系统
- 基于 ScriptableObject 实现事件驱动通信，解耦跨模块依赖（如场景加载、音效播放）
- 核心脚本：`Assets/Scripts/Event/SceneLoadEventSO.cs`、`Assets/Scripts/Event/PlayerAudioEventSO.cs`


## 运行说明
### 1. 本地运行（Unity 编辑器）
1. 环境要求：Unity 2022.3 及以上版本
2. 操作步骤：
   - 克隆本仓库到本地：`git clone https://github.com/xiaotaoxu2-afk/Lost-Realms.git`
   - 打开 Unity Hub，点击「添加项目」，选择克隆后的项目文件夹
   - 等待项目导入完成后，打开 `Assets/Scenes/StartScene.unity`，点击运行按钮即可启动游戏


### 3. 操作按键
| 操作         | 键盘按键 |
|--------------|----------|
| 移动         | WASD     |
| 普通攻击     | J        |
| 跳跃         | 空格    |
| 技能 1       | L        |
| 暂停/打开菜单 | Esc      |

## 项目结构
- Lost-Realms/
  - Assets/                // Unity 核心资源
    - Scripts/            // 核心 C# 脚本
      - PlayerAction/          // 玩家相关脚本
      - Enemy/           // 敌人相关脚本
      - ScriptableObject/           // 事件系统脚本（ScriptableObject 事件）
      - Audio/           //音效相关脚本
      - General/          //通用类脚本
      - Sava Load/        //数据保存加载类脚本
      - Transform/         //场景加载传送类脚本
      - UI/              // UI 相关脚本（面板管理、HUD 显示）
      - utilitys/            // 工具类脚本
    - Prefabs/            // 预制体（玩家、敌人、UI 组件）
    - Scenes/             // 关卡场景
  - Docs/                  // 技术文档
    - 架构设计.md         // 项目分层架构、模块划分、交互逻辑
    - 性能优化.md         // 优化背景、措施、量化成果
    - 已知问题与优化.md   // 现有小 bug、后续优化计划
  - .gitignore             // Unity 项目 Git 过滤规则
  - README.md              // 项目总览（本文档）

## 已知问题
1. 切换窗口/全屏时，UI 部分元素轻微错位（推荐使用 1080P 分辨率运行）
2. 普通敌人动画偶尔出现 1-2 帧卡顿（后续计划通过对象池优化）
3. 详细问题与优化计划见：`Docs/已知问题与优化.md`
