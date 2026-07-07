---
name: context-game-lib-gameverses
description: game-lib-gameverses submodule — Photon-based networking/multiplayer and Gameverses backend API
metadata:
  type: repo
  repo: game-lib-gameverses
  path: .
---

# Context: game-lib-gameverses (submodule)

- **Workspace mount:** `Assets/Code/Libs/game-lib-gameverses`
- **Repo:** git@github.com:drawcode/game-lib-gameverses.git (tracks `dev`)
- **Purpose:** Networking/multiplayer layer ("game network" lib) — Photon-based multiplayer, community/social features, and the Gameverses backend API.

## Structure (`Gameverses/`)
- `GameNetworking.cs`, `GameNetworkingPhoton.cs`, `GameNetworkingUnity.cs`, `GameNetworkingUnityPhoton.cs` — networking abstraction with Photon and Unity backends.
- `GameNetworkPhotonRPC.cs`, `GameNetworkPlayerContainer.cs` — RPC + networked player wrappers.
- `GameMessenger.cs` — messaging.
- `GameversesService.cs`, `GameversesGameAPI.cs`, `GameversesTypes.cs`, `GameversesGameObject.cs` — backend service API.
- **Community/** — social/community features.
- **Sync/** — state synchronization.

Consumed by the app's `GameMatchup`/`GameSocialGame` (`Assets/Code/Game/Game/Networking/`) and gated by `ENABLE_FEATURE_NETWORKING`. Works against the vendored Photon PUN2/Realtime/Chat SDKs in `Assets/Photon/`. Asmdef disabled (`.1asmdef`).
