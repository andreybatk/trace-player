import { Component } from '@angular/core';
import { ApiKeyService } from '../../data/services/api-key.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-admin-api-key',
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-api-key.component.html',
  styleUrl: './admin-api-key.component.scss'
})
export class AdminApiKeyComponent {
  serverIp: string = '';
  serverIps: string[] = [];
  message: string = '';

  constructor(private apiKeyService: ApiKeyService) {}

  ngOnInit(): void {
    this.loadServerIps();
  }

  loadServerIps(): void {
    this.apiKeyService.getAllServerIps().subscribe({
      next: (ips) => this.serverIps = ips,
      error: () => this.message = 'Ошибка при загрузке IP-адресов'
    });
  }

  addApiKey(): void {
    if (!this.serverIp) return;

    this.apiKeyService.addApiKey(this.serverIp).subscribe({
      next: (key) => {
        this.message = `API-ключ: ${key}`;
        this.loadServerIps();
        this.serverIp = '';
      },
      error: () => this.message = 'Ошибка при добавлении ключа'
    });
  }

  deleteApiKey(ip: string): void {
    const confirmed = confirm(`Вы уверены, что хотите удалить API-ключ для ${ip}?`);
    if (!confirmed) return;

    this.apiKeyService.deleteApiKey(ip).subscribe({
      next: () => {
        this.loadServerIps(); // Обновляем список IP
        this.message = `Ключ удалён для ${ip}`;
      },
      error: () => {
        this.message = 'Ошибка при удалении ключа';
      }
    });
  }
}
