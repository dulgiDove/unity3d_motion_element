# Unity 3D 기반 근접 액션 및 원소 반응 시스템

## 프로젝트 소개
Unity 3D 기반의 근접 액션 및 원소 반응 시스템 프로토타입입니다.

애니메이션 이벤트를 활용한 전투 시스템과,
불 / 물 / 전기 원소 간의 상호작용 시스템 구현에 중점을 두고 제작했습니다.

## 핵심 기능
- 3단 콤보 근접 전투
- Hitbox / Hurtbox 기반 타격 판정
- HitStop 및 CameraShake를 통한 타격감 구현
- 물체 집기 및 상호작용 시스템
- 불 / 물 / 전기 원소 시스템
- Steam / Overload / ElectroCharged 반응 구현

## 전투 시스템
전투 시스템은 Hitbox / Hurtbox 구조를 기반으로 구현했습니다.

각 공격의 실제 판정 타이밍은 Unity Animation Event를 통해 제어했습니다.

- 공격 애니메이션의 특정 프레임에서만 Hitbox 활성화
- 부위별 Hitbox 분리 (오른손 / 왼팔꿈치 / 발차기)
- 공격 성공 시 HitStop 및 CameraShake 적용

이를 통해 애니메이션과 전투 로직을 분리하면서도,
정확한 공격 타이밍을 구현할 수 있도록 설계했습니다.

<img width="984" height="838" alt="Unity_XhKY4abEZy" src="https://github.com/user-attachments/assets/674179ae-b140-4727-80fa-01c1040fa473" />


3단 콤보 어택 gif입니다.

<img width="727" height="707" alt="gitbox gizmo" src="https://github.com/user-attachments/assets/fef1dbdb-6812-49ec-ae80-fe885b7333e1" />


각 공격 부위의 hitbox gizmo를 구분하였습니다.


## 원소 시스템

원소 시스템은 역할 분리를 중심으로 설계했습니다.
원소 충돌 감지, 반응 판정, 실제 실행 책임을 각각 분리하여 구현했습니다

```text
[ElementSource]
원소를 제공하는 오브젝트
↓
[ElementReceiver]
외부 원소와의 충돌 및 상호작용 감지
↓
[ElementReactionResolver]
현재 원소와 입력 원소를 기반으로 반응 종류 판정
↓
[ElementReactionManager]
반응 효과 및 데미지 처리 실행
↓
[Reaction Effect]
Steam / Overload / ElectroCharged
```

### ElementReceiver
외부 원소와의 충돌 및 상호작용을 감지합니다.

### ElementReactionResolver
현재 원소와 입력 원소를 기반으로 어떤 반응이 발생하는지 계산합니다.

예시:
- Fire + Water → Extinguish
- Fire + Electricity → Overload
- Water + Electricity → ElectroCharged

### ElementReactionManager
반응의 실제 실행을 담당합니다.

- Steam VFX 생성
- 폭발 데미지 처리
- ElectroCharged 지속 데미지 처리

반응 계산과 실제 실행을 분리하여,
새로운 원소 반응을 추가하거나 수정하기 쉽도록 구조화했습니다.

<img width="984" height="838" alt="Unity_pIsVFthJpb" src="https://github.com/user-attachments/assets/704eb051-15d7-4fa7-a175-f0c6cb39139f" />


물건을 집는 모션과 
불 원소와 전기 원소가 충돌시에 폭발이 일어나는 gif입니다.

## 트러블슈팅

초기에는 각 오브젝트가 개별적으로 원소 반응을 처리하도록 구현했지만,
반응 로직이 여러 곳에 중복되며 유지보수가 어려워지는 문제가 있었습니다.

이를 해결하기 위해,
반응 계산은 ElementReactionResolver,
실제 실행은 ElementReactionManager로 분리했습니다.

그 결과 책임 분리를 통해 유지보수성과 리액션 확장 용이성을 향상시킬 수 있었습니다.
