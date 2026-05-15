# 2. Analysis

> **Project Title:** MULTI MINI ARCADE
> 
> <img src="./Images/TitleImage.png" width="800">


 **Student No / Name / E-mail:** (22113291, 강헌구, hungu020717@gmail.com)

 https://github.com/Hungu39/OpensourceSW-project

 
<br>

## [ Revision history ]

| Revision date | Version # | Description | Author |
| :--- | :--- | :--- | :--- |
| 05/03/2026 | 0.1 | 최초 초안 작성 | 강헌구 |
| 05/05/2026 | 0.2 | Use case 다이어그램 및 Usecase, 타이틀 이미지 추가 | 강헌구 |
| 05/06/2026 | 0.3 | UI 프로토타입 추가 | 강헌구 |

<br>

## = Contents =
1. [Introduction](#1-introduction)
2. [Use case analysis](#2-use-case-analysis)
3. [Domain analysis](#3-domain-analysis)
4. [User Interface prototype](#4-user-interface-prototype)
5. [Glossary](#5-glossary)
6. [References](#6-references)

---

## 1. Introduction

### 1. Summary
현대 사람들은 친구들과 함께 어울리며 즐길 수 있는 멀티플레이 게임을 선호한다. 하지만 최근의 네트워크 게임들은 복잡한 시스템과 긴 플레이 타임을 요구하여, 가볍고 빠르게 내기나 경쟁을 즐기기에는 부담스러운 경우가 많다. 이에 복잡한 룰이나 성장 요소 없이 오직 플레이어의 '순수 피지컬'만을 겨루며 직관적이고 빠른 재미를 추구하는 사람들을 위해 기획한 게임이 바로 "MULTI MINI ARCADE"이다.

### 2. Introduce "MULTI MINI ARCADE"
이번에 제작하게 된 게임 "MULTI MINI ARCADE"는 포톤(Photon) 네트워크를 기반으로 한 2인용 멀티플레이 미니게임 모음집이다. 해당 게임은 단순하지만 확실한 경쟁 요소를 가지는 '반응속도 대결', '에이밍 대결', '타자 대결' 총 3가지의 직관적인 미니게임으로 구성된다. 플레이어들은 로비 시스템을 통해 방을 생성하거나 참가하여 1:1 매칭을 진행하며, 짧은 시간 안에 서로의 순발력과 정확도를 측정하고 승패를 가르는 순수한 경쟁의 재미를 제공한다.

### 3. Goal
이번 Analysis 보고서에서는 Use case analysis와 Domain analysis을 진행하고 시스템이 어떻게 구성되었는가를 소개한다. 해당 보고서를 읽고 나면 "MULTI MINI ARCADE"의 멀티플레이 로비 세션 관리부터 각 미니게임 플레이 로직, 그리고 점수 동기화까지 전체적인 네트워크 게임 시스템이 어떤 방식으로 진행되고 동작하게 되는지 알 수 있을 것이다.

---

## 2. Use case analysis
<img src="./Images/usecase.png" width="800" alt="유스케이스 다이어그램">

### Use case #1 : Connection MasterServer
| GENERAL CHARACTERISTICS | |
| :--- | :--- |
| **Summary** | 게임플레이어가 멀티플레이게임을 하기위해 서버에 접속하기 위한 기능 |
| **Scope** | Multi-Mini Arcade |
| **Level** | user level |
| **Author** | |
| **Last Update** | |
| **Status** | Analysis |
| **Primary Actor** | Player |
| **Preconditions** | Player가 멀티 플레이 메뉴를 선택한 상황이여야 한다. |
| **Trigger** | Player가 멀티플레이 서버에 접속하려고 할 경우 |
| **Success Post Condition** | Player가 네트워크 상에 있으면(포톤 서버에 접속할 수 있으면) 접속에 성공한다 |
| **Failed Post Condition** | Player가 오프라인 상황에 있으면 (포톤 서버에 접속할 수 없을 경우) 접속에 실패한다. |
| **MAIN SUCCESS SCENARIO** | |
| **step** | **Action** |
| s | Player가 멀티플레이 서버에 접속한다. |
| 1 | 이 User case는 Player가 멀티플레이 메뉴를 선택할 때 시작된다. |
| 2 | Player가 원하는 방에 들어가거나 방을 만든다. |
| 3 | Player가 네트워크상에 존재한다면 서버에 접속한다. |
| 4 | 이 User case는 Player가 멀티플레이 서버 접속에 성공하면 끝난다. |
| **EXTENSION SCENARIOS** | |
| **step** | **Branching Action** |
| 3 | 3a. Player가 오프라인 상황이라면 접속에 실패한다.<br>...3a1. 접속에 실패했다는 문구를 보여준다.<br>...3a2. 다시 접속을 시도한다. |
| **RELATED INFORMATION** | |
| **Performance** | < 10 Seconds |
| **Frequency** | 플레이어가 게임 접속을 시도하는 만큼 |
| **\<Concurrency\>** | 제한없음 |
| **Due Date** | |

<br>

### Use case #2 : SetNickname
| GENERAL CHARACTERISTICS | |
| :--- | :--- |
| **Summary** | 플레이어가 게임 진입 전 사용할 자신의 닉네임을 입력하고 설정하는 기능 |
| **Scope** | Lobby system |
| **Level** | user level |
| **Author** | |
| **Last Update** | |
| **Status** | Analysis |
| **Primary Actor** | Player |
| **Preconditions** | 게임을 실행하여 초기 타이틀 화면에 진입한 상태여야 한다. |
| **Trigger** | Player가 닉네임 입력 필드에 텍스트를 작성하고 확인 버튼을 누른다. |
| **Success Post Condition** | 입력한 닉네임이 시스템(Photon Network)에 등록된다. |
| **Failed Post Condition** | 닉네임 설정이 거부되고 초기 화면에 머무른다. |
| **MAIN SUCCESS SCENARIO** | |
| **step** | **Action** |
| s | Player가 닉네임 입력 필드를 클릭한다. |
| 1 | Player가 사용할 닉네임을 키보드로 타이핑한다. |
| 2 | Player가 '확인' 버튼을 누른다. |
| 3 | 시스템은 입력된 문자열의 유효성(빈 칸 여부, 글자 수 등)을 검사한다. |
| 4 | 이 usecase는 유효성 검사를 통과한 닉네임이 서버(Photon)에 할당되고 로비 씬으로 넘어가면 끝난다. |
| **EXTENSION SCENARIOS** | |
| **step** | **Branching Action** |
| 3 | 3a. 닉네임 입력란이 비어있거나 공백만 있는 경우<br>...3a1. 시스템은 "닉네임을 입력해주세요"라는 경고 메시지를 띄운다.<br>3b. 지정된 최대 글자 수를 초과한 경우<br>...3b1. 글자 수가 초과되었다는 경고 메시지를 띄우고 입력을 거부한다. |
| **RELATED INFORMATION** | |
| **Performance** | < 0.5 Seconds |
| **Frequency** | Once per session |
| **\<Concurrency\>** | 제한없음 |
| **Due Date** | |

<br>

### Use case #3 : Create Room
| GENERAL CHARACTERISTICS | |
| :--- | :--- |
| **Summary** | 방장이 새로운 게임룸을 생성하고 해당 방에 입장하는 기능 |
| **Scope** | Lobby system |
| **Level** | user level |
| **Author** | |
| **Last Update** | |
| **Status** | Analysis |
| **Primary Actor** | Player |
| **Preconditions** | Player가 닉네임을 설정하고 포톤 메인 로비에 접속해 있는 상태여야 한다. |
| **Trigger** | Player가 로비 화면에서 '방 만들기' 버튼을 누른다. |
| **Success Post Condition** | 서버에 최대 인원이 2명으로 제한된 새로운 방이 생성되며, 생성한 Player가 방장(Master Client) 권한으로 방에 입장한다. |
| **Failed Post Condition** | 방 생성이 실패하고 로비 화면에 머무른다. |
| **MAIN SUCCESS SCENARIO** | |
| **step** | **Action** |
| s | Player가 로비에서 '방 만들기' 버튼을 누른다. |
| 1 | 시스템이 방 이름 입력 창을 띄운다. |
| 2 | Player가 방 이름을 입력하고 '생성' 버튼을 누른다. |
| 3 | 시스템은 입력된 방 이름과 최대 인원(2명) 설정값을 포함하여 포톤 서버에 방 생성 요청을 보낸다. |
| 4 | 서버가 방 생성을 승인하고 방 목록을 갱신한다. |
| 5 | 이 usecase는 Player가 생성된 방 내부(대기실 씬)로 이동하고 방장 권한을 부여받으면 끝난다. |
| **EXTENSION SCENARIOS** | |
| **step** | **Branching Action** |
| 2 | 2a. 방 이름을 입력하지 않고 '생성' 버튼을 누른 경우<br>...2a1. 시스템이 “이름을 입력하세요” 텍스트를 출력한다. |
| 4 | 4a. 이미 동일한 이름의 방이 존재하는 경우<br>...4a1. 시스템은 "이미 존재하는 방 이름입니다"라는 경고를 띄우고 1단계로 돌아간다. |
| **RELATED INFORMATION** | |
| **Performance** | < 1.0 Seconds |
| **Frequency** | frequent |
| **\<Concurrency\>** | 제한없음 |
| **Due Date** | |

<br>

### Use case #4 : Join Gameroom
| GENERAL CHARACTERISTICS | |
| :--- | :--- |
| **Summary** | Master Server에 접속한 player가 멀티플레이를 위한 Room에 접속하기 위한 기능 |
| **Scope** | Multi-Mini Arcade |
| **Level** | user level |
| **Author** | |
| **Last Update** | |
| **Status** | Analysis |
| **Primary Actor** | Player |
| **Preconditions** | Player가 MasterServer에 접속되어 있어야 한다. |
| **Trigger** | MasterServer에 접속한 Player가 게임룸에 참여하려고 할 때 |
| **Success Post Condition** | MasterServer에 존재하는 참여가 가능한 게임룸이 있다면 성공한다. |
| **Failed Post Condition** | MasterServer에 참여가 가능한 게임룸이 없는 경우 실패한다. |
| **MAIN SUCCESS SCENARIO** | |
| **step** | **Action** |
| s | Player가 게임룸에 참여한다. |
| 1 | 이 User case는 Player가 게임에 참여할 때 시작된다. |
| 2 | Player는 자신의 닉네임을 정하고 게임룸에 참여한다. |
| 3 | 현재 서버에 접속이 가능한 게임룸이 있다면 해당 게임룸에 접속한다. |
| 4 | 이 User case는 Player가 게임룸에 참여하면 끝난다. |
| **EXTENSION SCENARIOS** | |
| **step** | **Branching Action** |
| 2 | 2a. 설정한 닉네임을 입력하지 않는다면 실패한다.<br>...2a1. 닉네임의 글자 수가 부족하다는 안내 문구를 띄운다.<br>...2a2. 다시 접속을 시도한다. |
| 3 | 3a. 현재 서버에 접속이 가능한 게임룸이 없다면 실패한다.<br>...3a1. 참여 가능한 방이 없다는 안내 문구를 출력한다.|
| **RELATED INFORMATION** | |
| **Performance** | < 10 Seconds |
| **Frequency** | 플레이어가 게임 접속을 시도하는 만큼 |
| **\<Concurrency\>** | 제한없음 |
| **Due Date** | |

<br>

### Use case #5 : Start Game
| GENERAL CHARACTERISTICS | |
| :--- | :--- |
| **Summary** | 함께 플레이할 플레이어가 게임룸에 접속한 상황에 host player가 게임을 시작할 때 필요한 기능 |
| **Scope** | Multi-Mini Arcade |
| **Level** | user level |
| **Author** | |
| **Last Update** | |
| **Status** | Analysis |
| **Primary Actor** | Player |
| **Preconditions** | 게임을 시작하는 플레이어는 방장이어야 하고 모든 플레이어가 게임룸에 참여한 상황이어야 한다. |
| **Trigger** | 게임을 시작하려고 할 때 |
| **Success Post Condition** | 게임룸에 참여한 모든 플레이어가 게임 씬으로 이동한다. |
| **Failed Post Condition** | 플레이어가 게임 씬으로 이동하지 못한다. |
| **MAIN SUCCESS SCENARIO** | |
| **step** | **Action** |
| s | host Player가 게임룸에 참여한 모든 플레이어를 게임 씬으로 이동시키고 싶을 때 시작한다. |
| 1 | 게임 로비씬에서 플레이어가 모두 게임룸에 참여했는지 확인한다. |
| 2 | hostPlayer가 GameStart 버튼을 누른다. |
| 3 | 시스템은 게임룸에 참여한 모든 플레이어를 게임 씬으로 이동시킨다. |
| 4 | 이 usecase는 플레이어가 성공적으로 게임씬으로 이동하면 끝난다. |
| **EXTENSION SCENARIOS** | |
| **step** | **Branching Action** |
| 2 | 2a. 통신 문제 혹은 게임 씬에 문제가 생겨 게임 씬으로 이동할 수 없는 경우<br>...2a1. 실패 메시지를 띄운다. |
| **RELATED INFORMATION** | |
| **Performance** | < 10 Seconds |
| **Frequency** | 플레이어가 시도하는 만큼 |
| **\<Concurrency\>** | 제한없음 |
| **Due Date** | |

<br>

### Use case #6 : ReactionSpeedTest
| GENERAL CHARACTERISTICS | |
| :--- | :--- |
| **Summary** | 화면의 색상 변화 신호에 가장 빠르게 반응하여 버튼을 눌러 더 빨리 누른 사람이 승리하는 기능 |
| **Scope** | Mini Game 1 |
| **Level** | user level |
| **Author** | |
| **Last Update** | |
| **Status** | Analysis |
| **Primary Actor** | Player |
| **Preconditions** | 모든 플레이어가 게임이 시작된 상태여야 하고 GameManager에 의해 현재 라운드로 '반응속도 대결'이 선택되어야 한다. |
| **Trigger** | 화면의 색상이 빨간색으로 바뀐다. |
| **Success Post Condition** | 가장 먼저 버튼을 누른 Player의 입력이 서버에 반영되어 승리 처리된다. |
| **Failed Post Condition** | Player의 입력이 무시되거나 오입력으로 처리된다. |
| **MAIN SUCCESS SCENARIO** | |
| **step** | **Action** |
| s | Player가 화면을 주시하며 화면의 신호 변경을 대기한다. |
| 1 | 시스템은 무작위 대기 시간 후 화면의 신호를 변경한다. |
| 2 | Player는 신호를 확인하고 반응 버튼을 누른다. |
| 3 | 마스터 클라이언트는 입력 도달 시간을 비교하여 가장 빠른 입력을 판별한다. |
| 4 | 이 usecase는 승리한 Player에게 점수가 부여되고 결과가 동기화되면 끝난다. |
| **EXTENSION SCENARIOS** | |
| **step** | **Branching Action** |
| 2 | 2a. 신호가 변경되기 전에 Player가 버튼을 누른 경우 (부정 출발)<br>...2a1. 해당 Player는 이번 라운드에서 입력 불가 상태가 된다는 메시지를 띄운다. |
| 3 | 3a. 신호가 변경되도 긴 시간동안 버튼을 누르지 않은 경우<br>...3a1. 해당 Player는 실격되어 자동으로 패배 처리 |
| **RELATED INFORMATION** | |
| **Performance** | < 0.1 Seconds |
| **Frequency** | frequent |
| **\<Concurrency\>** | 제한없음 |
| **Due Date** | |

<br>

### Use case #7 : AimingTest
| GENERAL CHARACTERISTICS | |
| :--- | :--- |
| **Summary** | 각 플레이어의 화면에 독립적으로 생성되는 표적을 마우스로 조준하고 클릭하여 파괴해 점수를 겨루는 기능 |
| **Scope** | Mini Game 2 |
| **Level** | user level |
| **Author** | |
| **Last Update** | |
| **Status** | Analysis |
| **Primary Actor** | Player |
| **Preconditions** | player가 게임룸에 참여하여 게임이 시작된 상태여야 하고 GameManager에 의해 현재 라운드로 '에이밍 대결'이 선택되어야 한다. |
| **Trigger** | 각 플레이어의 게임 화면 내 무작위 좌표에 독립적인 표적 오브젝트가 생성된다. |
| **Success Post Condition** | Player의 클릭이 표적에 적중하여 표적이 파괴되고 개인 점수가 갱신된다. |
| **Failed Post Condition** | Player가 클릭을 안 하거나 클릭이 빗나가 표적이 파괴되지 않는다. |
| **MAIN SUCCESS SCENARIO** | |
| **step** | **Action** |
| s | Player가 화면을 주시하며 표적 생성을 대기한다. |
| 1 | 시스템은 각 Player의 화면에 독립적으로 랜덤한 위치에 표적들을 생성한다. |
| 2 | Player는 마우스를 조작하여 자신의 화면에 나타난 표적을 조준하고 좌클릭한다. |
| 3 | 클릭한 위치에 표적이 존재하면 표적 오브젝트를 파괴(비활성화)하고 개인 점수를 갱신한다. |
| 4 | 갱신된 점수는 네트워크를 통해 실시간으로 상대방 Player에게 동기화되어 UI에 표시된다. |
| 5 | 이 usecase는 지정된 제한 시간(또는 지정된 표적 개수)이 종료되어 승패 판정을 위한 최종 점수가 확정되면 끝난다. |
| **EXTENSION SCENARIOS** | |
| **step** | **Branching Action** |
| 2 | 2a. 표적이 아닌 빈 배경을 클릭한 경우<br>...2a1. 오발 이펙트가 발생하며 점수를 획득하지 못한다. (또는 연속 적중 콤보가 초기화된다.) |
| 3 | 3a. 표적이 생성된 후 일정 시간 내에 파괴하지 못한 경우<br>...3a1. 표적이 자연 소멸하며 점수를 획득하지 못하고 다음 표적이 새롭게 생성된다. |
| **RELATED INFORMATION** | |
| **Performance** | < 0.1 Seconds |
| **Frequency** | frequent |
| **\<Concurrency\>** | Medium |
| **Due Date** | |

<br>

### Use case #8 : TypingTest
| GENERAL CHARACTERISTICS | |
| :--- | :--- |
| **Summary** | 화면에 출력된 제시어를 오타 없이 가장 먼저 입력하여 제출하는 기능 |
| **Scope** | Mini Game 3 |
| **Level** | user level |
| **Author** | |
| **Last Update** | |
| **Status** | Analysis |
| **Primary Actor** | Player |
| **Preconditions** | player가 게임룸에 참여하여 게임이 시작된 상태여야 하고 GameManager에 의해 현재 라운드로 '타자 대결'이 선택되어야 한다. |
| **Trigger** | 시스템이 화면 중앙에 텍스트 형태의 제시어를 출력한다. |
| **Success Post Condition** | Player가 제출한 텍스트가 제시어와 정확히 일치하여 성공 처리된다. |
| **Failed Post Condition** | 오타가 포함되어 정답으로 인정되지 않는다. |
| **MAIN SUCCESS SCENARIO** | |
| **step** | **Action** |
| s | Player가 화면의 제시어 출력을 대기한다. |
| 1 | 시스템이 무작위 문장을 선택해 동일하게 각 플레이어의 화면에 표시한다. |
| 2 | Player는 입력 필드에 키보드로 제시어를 타이핑한다. |
| 3 | Player가 엔터 키를 눌러 입력값을 서버로 전송한다. |
| 4 | 시스템이 원본 제시어와 Player의 입력 문자열이 완전히 일치하는지 검증한다. |
| 5 | 이 usecase는 가장 먼저 정확하게 입력한 Player가 판별되어 점수를 획득하면 끝난다. |
| **EXTENSION SCENARIOS** | |
| **step** | **Branching Action** |
| 4 | 4a. 오타나 띄어쓰기 오류로 인해 입력 문자열이 일치하지 않는 경우<br>...4a1. 오답 이펙트를 출력하고 오타 부분을 빨갛게 표시한다. |
| **RELATED INFORMATION** | |
| **Performance** | < 0.1 Seconds |
| **Frequency** | frequent |
| **\<Concurrency\>** | 제한없음 |
| **Due Date** | |

<br>

### Use case #9 : View Result
| GENERAL CHARACTERISTICS | |
| :--- | :--- |
| **Summary** | 3개의 미니게임 라운드가 모두 종료된 후, 플레이어의 최종 점수와 승패 결과를 화면에 출력하는 기능 |
| **Scope** | Game system |
| **Level** | user level |
| **Author** | |
| **Last Update** | 2026-05-05 |
| **Status** | Analysis |
| **Primary Actor** | Player |
| **Preconditions** | 3가지 미니게임(반응속도, 에이밍, 타자)이 모두 정상적으로 완료된 상태여야 한다. |
| **Trigger** | 마지막 미니게임의 점수 집계 및 동기화가 완료되었을 때 |
| **Success Post Condition** | 최종 점수와 승패 결과가 UI에 올바르게 출력되고, 로비 복귀 등의 다음 단계로 넘어갈 준비가 완료된다. |
| **Failed Post Condition** | 네트워크 오류 등으로 결과 창이 정상적으로 뜨지 않는다. |
| **MAIN SUCCESS SCENARIO** | |
| **step** | **Action** |
| s | 모든 미니게임 종료 후, 시스템이 결과 정산 로직을 시작한다. |
| 1 | 시스템(마스터 클라이언트)은 두 플레이어의 누적 점수를 비교하여 최종 승자와 패자를 판별한다. |
| 2 | 시스템은 결과 화면 UI 패널을 활성화한다. |
| 3 | 각 Player의 화면에 본인의 점수, 상대방의 점수, 그리고 최종 승패 결과 텍스트(Win/Lose)가 표시된다. |
| 4 | 시스템은 일정 시간 대기 후(혹은 확인 버튼 클릭 대기 후) '로비로 돌아가기' 버튼을 활성화한다. |
| 5 | 이 usecase는 플레이어가 결과를 확인하고 결과 창 출력이 정상적으로 마무리되면 끝난다. |
| **EXTENSION SCENARIOS** | |
| **step** | **Branching Action** |
| 1 | 1a. 두 플레이어의 최종 누적 점수가 완전히 동일한 경우 (동점)<br>...1a1. 최종 결과를 '무승부(Draw)'로 판별하고 양쪽 플레이어 모두에게 무승부 결과를 출력한다. |
| 3 | 3a. 네트워크 지연으로 인해 점수 동기화가 지연된 경우<br>...3a1. "결과를 불러오는 중입니다..." 메시지를 띄우고 서버 데이터 동기화를 재시도한다. |
| **RELATED INFORMATION** | |
| **Performance** | < 1.0 Seconds |
| **Frequency** | 1회 (모든 게임이 끝날 때마다) |
| **\<Concurrency\>** | Medium |
| **Due Date** | |


<br>

### Use case #10 : Exit Game
| GENERAL CHARACTERISTICS | |
| :--- | :--- |
| **Summary** | 플레이어가 진행 중인 게임 룸에서 퇴장하여 로비로 돌아가거나 클라이언트를 종료하는 기능 |
| **Scope** | Multi-Mini Arcade |
| **Level** | user level |
| **Author** | |
| **Last Update** | 2026-05-05 |
| **Status** | Analysis |
| **Primary Actor** | Player |
| **Preconditions** | 플레이어가 게임룸에 접속해 있는 상태여야 한다. |
| **Trigger** | Player가 결과 화면이나 인게임 메뉴에서 '방 나가기' 또는 '종료' 버튼을 누를 때 |
| **Success Post Condition** | 포톤 서버의 방(Room) 연결이 해제되고, 플레이어가 메인 로비 화면으로 전환되거나 앱이 완전히 종료된다. |
| **Failed Post Condition** | 정상적인 퇴장 처리가 진행되지 않고 게임룸에 머무른다. |
| **MAIN SUCCESS SCENARIO** | |
| **step** | **Action** |
| s | Player가 '방 나가기' 버튼을 누른다. |
| 1 | 시스템은 포톤 네트워크 서버에 해당 Player의 방 퇴장(LeaveRoom) 요청을 보낸다. |
| 2 | 서버가 요청을 승인하고, 룸에 남은 상대방 플레이어에게 퇴장 알림을 보낸다. |
| 3 | Player의 캐릭터 및 관련 객체를 비활성화하고, 화면을 메인 로비 씬(Scene)으로 전환한다. |
| 4 | 이 usecase는 성공적으로 네트워크 세션 연결이 끊어지고 방에서 나가면 끝난다. |
| **EXTENSION SCENARIOS** | |
| **step** | **Branching Action** |
| 2 | 2a. 방장(Master Client)이 방을 먼저 나간 경우<br>...2a1. 게임룸이 더 이상 유지되지 않도록 방을 폭파(해산) 처리한다.<br>...2a2. 룸에 남아있던 상대방 Player에게 "상대방이 나가 게임이 종료되었습니다" 메시지를 띄우고 함께 로비로 강제 퇴장시킨다. |
| s | s1. 인게임(진행 중) 도중에 강제로 방 나가기를 누른 경우<br>...s1a. 해당 Player를 기권(패배) 처리하고 즉시 방에서 퇴장시킨다. |
| **RELATED INFORMATION** | |
| **Performance** | < 2.0 Seconds |
| **Frequency** | 게임룸을 나갈 때마다 |
| **\<Concurrency\>** | None |
| **Due Date** | |
---

## 3. Domain analysis

- **PlayerController**
  플레이어의 입력을 처리하는 클래스이다. 반응속도 대결의 버튼 입력, 에이밍 대결의 마우스 조준 및 클릭, 타자 대결의 키보드 입력 등 플레이어의 조작 정보를 받아 서버로 전달하는 역할을 한다.

- **LobbyManager**
  네트워크 서버에 접속한 플레이어에 대해 게임룸을 생성하고 해당 룸에 접속중인 모든 플레이어를 게임씬으로 이동시키는 등의 네트워크와 관련된 일을 하는 클래스이다. 최대 2명의 플레이어가 모였을 때 게임을 원활하게 시작할 수 있도록 방의 상태를 관리한다.

- **GameManager**
  게임 전체 흐름을 관리하는 클래스이다. 세 가지 미니게임(반응속도, 에이밍, 타자) 중 어떤 게임을 실행할지 순서를 결정하고, 모든 라운드가 종료되었을 때 최종 승패를 판정하여 결과를 출력하는 역할을 한다.

- **MiniGameBase**
  각 미니게임이 공통으로 상속받는 부모 클래스이다. 게임 시작 전 카운트다운, 게임 종료 이벤트, 공통 점수 계산 로직 등 미니게임 구동에 필요한 기본적인 뼈대 정보를 가지고 있다.

- **ScoreManager**
  각 플레이어가 미니게임에서 획득한 점수를 관리하는 클래스이다. 라운드별 점수를 누적 합산하고, 네트워크를 통해 상대방 클라이언트와 점수를 실시간으로 동기화하는 역할을 한다.

- **ReactionGame**
  반응속도 대결 미니게임의 내부 로직을 관리하는 클래스이다. 무작위 대기 시간 생성, 화면 신호 변경 이벤트 처리, 플레이어의 반응 속도 측정 및 승리 판정 기능을 가지고 있다.

- **AimingGame**
  에이밍 대결 미니게임의 내부 로직을 관리하는 클래스이다. 각 플레이어의 화면의 무작위 위치에 표적을 생성하고 플레이어가 클릭했을 때 점수가 오르고 실패하지않고 연속으로 누를 경우 콤보가 오르는 기능을 가지고 있다.

- **Target**
  에이밍 대결에서 화면에 생성되는 개별 표적 오브젝트를 관리하는 클래스이다. 표적의 생성 좌표(Vector), 파괴 시 획득할 점수, 활성화 상태 등의 정보를 가지고 있다.

- **TypingGame**
  타자 대결 미니게임의 내부 로직을 관리하는 클래스이다. 제시어 데이터베이스에서 무작위 단어나 문장을 불러와 화면에 띄우고, 플레이어가 제출한 문자열이 정답과 오타 없이 일치하는지 비교 검증하는 기능을 가지고 있다.

- **UIManager**
  플레이어의 ui와 관련된 정보를 보여주는 클래스이다. 현재 진행 중인 미니게임의 종류와 룰, 플레이어와 상대방의 실시간 점수 현황, 입력 성공 및 실패 이펙트, 최종 결과 화면 등을 렌더링하는 역할을 한다.

---

## 4. User Interface prototype
<img src="./Images/TitlePrototype.png" width="800">
플레이어가 처음 게임을 실행하면 나오는 화면이다. 닉네임을 입력하고 방 만들기를 누르면 방장으로 방을 만들 수 있고 방 참가를 누르면 만들어져 있는 방에 일반 유저로써 참여 가능하다. 

<img src="./Images/WaitingRoomPrototype.png" width="800">
플레이어가 방에 들어왔을 때 나오는 화면이다. 플레이어 2명의 대기상태와 게임 시작 버튼을 누를 수 있다. 

<img src="./Images/Aiming.jpg" width="800">
미니게임 중 에이밍 게임의 화면이다. 플레이어가 마우스로 조준점을 움직일 수 있고 랜덤한 위치에 표적이 순서대로 생겼다가 없어졌다가 할 것이다. 그 표적이 없어지기 전에 플레이어가 마우스로 클릭하면 점수가 오른다. 왼쪽 위에는 제한시간이 표시되고 오른쪽 위에는 상대의 점수가 표시된다. 아래에는 몇 번 연속으로 표적을 맞추는데 성공했는지 표시된다. 


<br>
위 이미지는 모두 AI로 생성된 이미지이다.


## 5. Glossary
| Term | Description |
| :--- | :--- |
| 마스터 클라이언트 플레이어 | 방을 처음 만들고 방장 권한을 가진 플레이어이다. 본 게임에서는 미니게임의 시작 제어와 플레이어 간의 입력 시간 비교(승패 판정) 등을 주도하는 역할을 한다. |
| 클라이언트 플레이어 | 마스터 클라이언트가 생성한 게임룸에 참여한 상대방 플레이어를 말한다. |
| 포톤 네트워크 (Photon Network) | 2인 멀티플레이어 환경(방 생성, 접속, 데이터 통신)을 구현하기 위해 사용하는 유니티용 네트워크 엔진을 말한다. |
| 로비 씬 | 플레이어들이 닉네임을 설정하고 방을 생성하거나 참가 대기하는 씬을 말한다. |
| 게임 씬 | 두 명의 플레이어가 실제로 3가지 미니게임(반응속도, 에이밍, 타자)을 진행하며 피지컬을 겨루는 씬을 말한다. |

## 6. References
1) Unity Documentation
https://docs.unity3d.com/kr/2023.2/Manual/Glossary.html
2) Photon Network
https://www.photonengine.com/
