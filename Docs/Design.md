> **Project Title:** MULTI MINI ARCADE
> 
> <img src="../Images/TitleImage.png" width="800">


 **Student No / Name / E-mail:** (22113291, 강헌구, hungu020717@gmail.com)

 https://github.com/Hungu39/OpensourceSW-project

<br>

## [ Revision history ]

| Revision date | Version # | Description | Author |
| :--- | :--- | :--- | :--- |
| 06/01/2026 | 0.1 | 클래스 다이어그램 작성 | 강헌구 |


<br>


---

## 1. Introduction

### 1. Summary
현대 사람들은 친구들과 함께 어울리며 즐길 수 있는 멀티플레이 게임을 선호한다. 하지만 최근의 네트워크 게임들은 복잡한 시스템과 긴 플레이 타임을 요구하여, 가볍고 빠르게 내기나 경쟁을 즐기기에는 부담스러운 경우가 많다. 이에 복잡한 룰이나 성장 요소 없이 오직 플레이어의 '순수 피지컬'만을 겨루며 직관적이고 빠른 재미를 추구하는 사람들을 위해 기획한 게임이 바로 "MULTI MINI ARCADE"이다.

### 2. Introduce "MULTI MINI ARCADE"
이번에 제작하게 된 게임 "MULTI MINI ARCADE"는 포톤(Photon) 네트워크를 기반으로 한 2인용 멀티플레이 미니게임 모음집이다. 해당 게임은 단순하지만 확실한 경쟁 요소를 가지는 '반응속도 대결', '에이밍 대결', '타자 대결' 총 3가지의 직관적인 미니게임으로 구성된다. 플레이어들은 로비 시스템을 통해 방을 생성하거나 참가하여 1:1 매칭을 진행하며, 짧은 시간 안에 서로의 순발력과 정확도를 측정하고 승패를 가르는 순수한 경쟁의 재미를 제공한다.

### 3. Goal
이번 Analysis 보고서에서는 Use case analysis와 Domain analysis을 진행하고 시스템이 어떻게 구성되었는가를 소개한다. 해당 보고서를 읽고 나면 "MULTI MINI ARCADE"의 멀티플레이 로비 세션 관리부터 각 미니게임 플레이 로직, 그리고 점수 동기화까지 전체적인 네트워크 게임 시스템이 어떤 방식으로 진행되고 동작하게 되는지 알 수 있을 것이다.

<br>

## 2. Class diagram

해당 클래스 다이어그램은 Multi-Mini Arcade의 로비, 대기방, 그리고 3종의 미니게임 및 결과 창을 제어하는 핵심 매니저(Manager) 클래스들을 표현한 다이어그램이다. 

실제 구현에는 UI나 자잘한 이펙트를 관리하는 스크립트들이 더 포함되지만, 본 다이어그램에서는 게임의 주요 로직과 네트워크 통신을 담당하는 핵심 요소들만 추려서 나타내었다. 다이어그램에 나타난 대부분의 매니저 클래스는 유니티 스크립트의 기본인 `MonoBehaviour`와 포톤 네트워크 제어를 위한 `MonoBehaviourPunCallbacks`를 상속받아 동작한다. 또한, 멀티플레이 특성상 클라이언트 간 동기화를 위해 다수의 `public` 메서드와 RPC(Remote Procedure Call) 메서드가 포함되어 있는 것이 특징이다.

기능별로 연관된 클래스들을 묶어서 설명한다.
