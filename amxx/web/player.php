<?php
  header("Content-Type: text/html; charset=UTF-8");

  $apiKey = "apiKey";
  $fungunApiKeyMd5 = "fungunApiKeyMd5";
  $steamApiKey = "steamApiKey";
  $steamId = $_GET['steamId'] ?? null;

  if (!$steamId) {
    echo "SteamID не указан";
    exit;
  }

  $apiUrl = "http://localhost:5000/api/player/bySteamId?steamId=$steamId";

  $options = [
    "http" => [
        "method" => "GET",
        "header" => "Accept: application/json\r\n" .
                    "X-API-Key: $apiKey\r\n" .
                    "X-STEAM-API-Key: $steamApiKey\r\n" .
                    "X-FUNGUN-API-Key: $fungunApiKeyMd5\r\n"
    ]
  ];

  $context = stream_context_create($options);
  $response = file_get_contents($apiUrl, false, $context);
  $data = json_decode($response, true);

  $info = $data['fullSteamPlayerInfo']['playerInfo'] ?? null;
  $ban = $data['fullSteamPlayerInfo']['banInfo'] ?? null;
  $game = $data['fullSteamPlayerInfo']['gameInfo'] ?? null;
  $names = $data['names'] ?? [];
  $fungunPlayer = $data['fungunPlayer'] ?? null;
?>


<!DOCTYPE html>
<html lang="ru">
<head>
  <meta charset="UTF-8">
  <title>Trace Player</title>
  <link rel="stylesheet" href="player.css">
</head>
<body>
  <div class="player-detail-container">
    <h2 class="player-detail-header">Trace Player</h2>

    <div class="player-info">
      <div class="player-info-left">
        <?php if ($info): ?>
          <h3>Steam</h3>
          <p><?= htmlspecialchars($info['personaname']) ?></p>
          <a href="<?= htmlspecialchars($info['profileurl']) ?>" target="_blank">
            <img src="<?= htmlspecialchars($info['avatarfull']) ?>" alt="Avatar" class="player-avatar">
          </a>
        <?php endif; ?>

        <h3>Steam ID</h3>
        <p>
          <?= htmlspecialchars($steamId) ?>
          <?php if ($data['countryCode']): ?>
          <img 
              src="https://flagcdn.com/24x18/<?php echo strtolower($data['countryCode']); ?>.png"
              alt="<?php echo htmlspecialchars($data['countryCode']); ?> flag"
              width="24"
              height="18"
              style="margin-right: 6px;"
          />
          <?php endif; ?>
        </p>
        <?php if ($game): ?>
          <h3>Counter-Strike</h3>
          <p><?= intval($game['playtime_forever'] / 60) ?> ч. всего</p>
          <p><?= intval($game['playtime_2weeks'] / 60) ?> ч. за последние 2 недели</p>
        <?php endif; ?>
      </div>

      <div class="player-info-middle">
        <?php if ($ban): ?>
          <h3>Steam Bans</h3>
          <p><strong>VAC Bans:</strong> <?= $ban['numberOfVACBans'] ?></p>
          <p><strong>Game Bans:</strong> <?= $ban['numberOfGameBans'] ?></p>
          <p><strong class="danger-bg"><?= $ban['communityBanned'] ? 'Community ban' : '' ?></strong></p>
        <?php endif; ?>
      </div>

      <?php if ($fungunPlayer): ?>
      <div class="player-info-right">
        <h3>Fungun ECD</h3>
        <?php if ($fungunPlayer['lastDanger']): ?>
          <p> 
            <strong class="status-danger">Обнаружены читы</strong> - </a>
            Report Id <?= htmlspecialchars($fungunPlayer['lastDanger']['report_id']) ?>
          </p>
        <?php elseif ($fungunPlayer['lastWarning']): ?>
          <p> 
            <strong class="status-warning">Подозрительно</strong> - </a>
            Report Id <?= htmlspecialchars($fungunPlayer['lastWarning']['report_id']) ?>
          </p>
        <?php elseif ($fungunPlayer['lastSuccess']): ?>
          <p> 
            <strong class="status-success">Чисто</strong> - </a>
            Report Id <?= htmlspecialchars($fungunPlayer['lastSuccess']['report_id']) ?>
          </p>
        <?php else: ?>
          <p>Нет данных по Fungun</p>
        <?php endif; ?>
      </div>
    <?php endif; ?>
    </div>
    
    <div class="player-history-container">
      <?php if (count($names)): ?>
        <div class="player-names-section">
          <h3>История имён</h3>
          <table class="player-table">
            <thead><tr><th>Имя</th><th>Сервер</th><th>Дата</th></tr></thead>
            <tbody>
              <?php foreach ($names as $name): ?>
                <tr>
                  <td><?= htmlspecialchars($name['name']) ?></td>
                  <td><?= htmlspecialchars($name['server']) ?></td>
                  <td><?= date('d.m.Y', strtotime($name['addedAt'])) ?></td>
                </tr>
              <?php endforeach; ?>
            </tbody>
          </table>
        </div>
      <?php endif; ?>
    </div>
  </div>
</body>
</html>