<?php
// Пример: /player.php?steamId=STEAM_1:1:12345678
header("Content-Type: text/html; charset=UTF-8");
$apiKey = "apiKey";
$steamId = $_GET['steamId'] ?? null;

if (!$steamId) {
    echo "SteamID не указан";
    exit;
}

$apiUrl = "http://localhost:5000/api/player/bySteamId?steamId=$steamId";
$options = [
    "http" => [
        "method" => "GET",
        "header" => "Accept: text/plain\r\n" .
                    "X-API-Key: $apiKey\r\n"
    ]
];

$context = stream_context_create($options);
$response = file_get_contents($apiUrl, false, $context);
$data = json_decode($response, true);

if (!$data) {
    echo "Игрок не найден";
    exit;
}

$info = $data['fullSteamPlayerInfo']['playerInfo'] ?? null;
$ban = $data['fullSteamPlayerInfo']['banInfo'] ?? null;
$names = $data['names'] ?? [];
$ips = $data['ips'] ?? [];

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
        <p><?= htmlspecialchars($steamId) ?></p>
      </div>

      <div class="player-info-middle">
        <?php if ($ban): ?>
          <h3>Steam Bans</h3>
          <p><strong>VAC Bans:</strong> <?= $ban['numberOfVACBans'] ?></p>
          <p><strong>Game Bans:</strong> <?= $ban['numberOfGameBans'] ?></p>
          <p><strong class="danger-bg"><?= $ban['communityBanned'] ? 'Community ban' : '' ?></strong></p>
        <?php endif; ?>
      </div>
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

      <?php if (count($ips)): ?>
        <div class="player-ips-section">
          <h3>История подключений</h3>
          <table class="player-table">
            <thead><tr><th>Страна</th><th>Дата</th></tr></thead>
            <tbody>
              <?php foreach ($ips as $ip): ?>
                <tr>
                  <td>
                    <?php if (!empty($ip['countryCode'])): ?>
                      <img src="http://flagcdn.com/24x18/<?= strtolower($ip['countryCode']) ?>.png" alt="flag">
                      <?= htmlspecialchars($ip['countryCode']) ?>
                    <?php endif; ?>
                  </td>
                  <td><?= date('d.m.Y', strtotime($ip['addedAt'])) ?></td>
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
