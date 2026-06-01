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

### [Lobby & Waiting Room System]
서버 접속부터 게임 시작 전까지의 준비 과정을 담당하는 클래스들이다.

| 클래스명 | 설명 |
| :--- | :--- |
| **LobbyManager** | 닉네임 설정, 유효성 검사, 방 생성 및 참가 등 로비 씬에서의 전반적인 네트워크 연결을 관리하는 클래스이다.<br><br>+ `OnClickCreateRoom() / OnClickJoinRoom()`: 유저의 UI 버튼 입력을 받아 방 생성 및 참가를 서버에 요청한다.<br>+ `OnJoinedRoom()`: 방 입장에 성공했을 때 대기방 씬으로 전환하는 콜백 메서드이다. |
| **WaitingRoomManager** | 게임룸에 입장한 플레이어들의 레디 상태를 관리하고, 인원이 모두 모였을 때 게임 씬으로 이동시키는 역할을 한다.<br><br>+ `SetReady()`: 플레이어의 준비 상태를 토글하는 메서드이다.<br>+ `OnPlayerEnteredRoom() / OnPlayerLeftRoom()`: 다른 유저가 들어오거나 나갈 때 UI를 갱신하고 예외를 처리한다. |
| **WaitingRoomChat** | 대기방 내부에서 플레이어 간 텍스트 채팅 기능을 수행하는 클래스이다.<br><br>+ `SendChatMessage()`: 입력된 텍스트를 서버로 전송한다.<br>+ `RPC_ReceiveChat()`: 상대방이 보낸 메시지를 수신하여 로컬 UI 화면에 업데이트한다. |

<br>

### [Mini Game Managers]
실제 3가지 미니게임 씬 내부의 규칙, 타이머, 승패 판정을 제어하는 클래스들이다.

| 클래스명 | 설명 |
| :--- | :--- |
| **ReactionGameManager** | '반응속도 대결' 미니게임의 내부 로직을 관리하는 클래스이다. 무작위 대기 시간 생성, 화면 신호 변경, 플레이어 반응 측정 기능을 가진다.<br><br>+ `RandomWaitCoroutine()`: 신호가 변경되기 전까지의 무작위 대기 시간을 제어한다.<br>+ `OnScreenClicked()`: 플레이어의 클릭 입력을 감지한다.<br>+ `RPC_SubmitTime()`: 클릭이 유효할 경우, 서버로 자신의 반응 속도를 제출하고 동기화한다. |
| **AimingGameManager** | '에이밍 대결' 미니게임의 내부 로직을 관리하는 클래스이다. 표적의 스폰과 클릭 판정, 제한 시간을 제어한다.<br><br>+ `SpawnTarget()`: 화면 내 무작위 위치에 표적을 생성한다.<br>+ `OnTargetClicked()`: 표적을 성공적으로 클릭했을 때 로컬 점수를 올린다.<br>+ `RPC_AddAimingScore()`: 획득한 점수를 상대방 클라이언트에도 동기화시킨다. |
| **TypingGameManager** | '타자 대결' 미니게임의 로직을 관리한다. 제시어를 띄우고 유저의 입력이 정확한지 검증하는 클래스이다.<br><br>+ `SpawnInitialWords()` / `RPC_SyncInitialWords()`: 양쪽 플레이어의 화면에 동일한 제시어가 나타나도록 단어 데이터를 세팅하고 동기화한다.<br>+ `OnInputSubmit()`: 유저가 제출한 단어의 정답 여부를 판별한다.<br>+ `RPC_StealWord()`: 정답을 맞힌 경우 단어의 소유권을 가져오고 점수를 갱신한다. |

<br>

### [Score & Result System]
게임 전반에 걸친 점수 누적과 최종 결과 출력을 담당하는 클래스들이다.

| 클래스명 | 설명 |
| :--- | :--- |
| **TotalScoreManager** | 3개의 라운드가 진행되는 동안 각 미니게임에서 획득한 승점을 누적하여 저장하는 데이터 클래스이다.<br><br>+ `hostTotalScore / guestTotalScore`: 방장과 게스트의 누적 승점을 저장하는 변수이다.<br>+ `ResetScores()`: 모든 게임이 끝나고 다음 게임을 위해 누적 점수를 초기화한다. |
| **FinalResultManager** | 모든 미니게임 라운드가 종료된 후 최종 승패를 계산하여 UI에 출력하고 씬을 제어하는 클래스이다.<br><br>+ `Start()`: 씬 로드 시 TotalScoreManager의 데이터를 바탕으로 승자를 판별한다.<br>+ `ReturnToWaitingRoom()`: 결과를 일정 시간 출력한 뒤 자동으로 대기방 씬으로 플레이어들을 복귀시킨다. |
