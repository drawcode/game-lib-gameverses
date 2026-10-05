---
name: context-community-social
description: game-lib-gameverses Community layer — an optional custom REST leaderboard/stat/sync backend (GameCommunity + GameCommunityService over AppConfigs.apiUrlWeb) plus Facebook/Twitter share, independent of GameCenter/Play Games. The cross-platform/social alternative to native networks. Referenced by context-profile-progression-rpg-shared.
metadata:
  type: reference
  repo: game-lib-gameverses
  created: 2026-07-21
---

# game-lib-gameverses — Community / social backend

Optional third leg of the shared progression story: a **custom cross-platform
leaderboard/stat/sync service + social share**, separate from the native
GameCenter/Play Games path ([[context-progression-runtime-networks]]). Reads/writes
the same local `GameProfileStatistics`/`GameProfileAchievements`
([[context-profile-progression-data]]). Use when a title wants social/cross-
platform leaderboards beyond native. Overview: [[context-profile-progression-rpg-shared]].

## `GameCommunity` (`Community/GameCommunity.cs`, plain singleton)
Public facade. Header comment documents the intended API.
- **Login** — Facebook/Twitter only (`SocialNetworkTypes`), delegates to
  `SocialNetworks.ShowLoginFacebook/Twitter`; per-network `GameCommunityNetworkLoginState`.
  **No GameCenter/Play Games login here.**
- **Stats/achievements (local)** — `Set/GetStatisticValue*`, `SetAchievementValue`
  over `GameProfileStatistics/Achievements` + `GamePlayerProgress.Instance`; each set
  queues a sync via `GameCommunityTrackingController.SetSyncStatistic` + tracks an event.
- **High score / points / rank** — keyed off `AppConfigs.socialStatisticForFacebook`
  (single aggregated stat, since Facebook Scores allows one score/app) +
  `GameCommunityStatisticCodes`.
- **Leaderboards (custom)** — `RequestLeaderboards(code,page,size,type)` /
  `RequestLeaderboardUser` → `GameCommunityService`; `RequestLeaderboardFriends` →
  Facebook Graph. `GameLeaderboardType` ALL/DAILY/WEEKLY/MONTHLY.
- **Sync** — `SyncProfileProgress`→`sendSync()`: if FB enabled+logged-in posts the
  single high-score to Facebook; if `featureEnableCustomLeaderboards` builds a
  `GameCommunitySyncData` (network user, diffed stats/achievements, profile+system
  attrs) → `GameCommunityService.SyncData`.

## Controllers
- **`GameCommunityController`** (MonoBehaviour) — Messenger glue: on FB login likes
  app + `SyncProfileProgress`; `OnProfileData` parses Graph `me` into
  `GameProfiles.Current.SetNetworkValue*` + `GameProfileTrackers`; leaderboard-data
  handlers update ranks by matching username; `LeaderboardsShowOnline` →
  `Platforms.ShowWebView(AppConfigs.appUrlWeb)`.
- **`GameCommunitySocialController`** — screenshot capture (`ScreenCapture`,
  `SaveImageToLibrary` via Etcetera) + share flow (`StartPhotoUploadToFacebook/
  Twitter`, `PostGameResults`) building context-aware text from current UI panel +
  `GameController` runtime data.
- **`GameCommunityService`** (plain singleton) — the **custom REST client**. Base
  `AppConfigs.apiUrlWeb`; routes `{apiUrl}/game/{socialGameCommunityAppCode}/{action}/`
  (`leaderboard`, `sync`). Every request posts `profileId=GameProfiles.Current.uuid`
  + `auth=AppConfigs.socialGameCommunityAppAuth`. `GetLeaderboardFull/User` →
  broadcast `gameCommunityLeaderboardData/UserData`; `SyncData` POST → `gameCommunitySyncComplete`.
  Via `Engine.Networking.WebRequests.Instance.Request`.

## Types (`Community/Types/GameCommunityTypes.cs`)
`GameCommunityStatisticCodes` (points, high-score, total-score, time-played,
times-played, shots-made/missed, top-rank, current-rank[-total]);
`GameCommunityLeaderboardData`/`Item`; `GameCommunitySyncData` (+ User /
ProfileStatistics / Achievements / DataUpdates); `GameCommunityNetworkUser`;
`GameCommunityMessages`; `GameCommunitySystemTracking` (device/OS telemetry,
`platformCode` ios/android/desktop).

## Config a title supplies (only if community used)
`AppConfigs.apiUrlWeb`, `socialGameCommunityAppCode`, `socialGameCommunityAppAuth`,
`socialGameCommunityAppId`, `socialStatisticForFacebook`, Facebook/Twitter app
ids+secrets, `appUrlScheme`. Small titles can skip this whole layer and rely on
native networks only.

## UI panels
`Community/UI/GameCommunityUIPanel{Statistics,Achievements,Leaderboards}` — render
definition+profile lists (achievements sum `data.points`, dim incomplete α .33;
leaderboards render the `"high-score"` bucket).
