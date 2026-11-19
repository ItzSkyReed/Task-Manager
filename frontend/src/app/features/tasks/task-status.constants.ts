import { TaskItemStatus } from '../../core/models/api.models';

// Элемент для списков
export interface TaskStatusOption {
  value: TaskItemStatus;
  label: string;
}

// Список всех доступных статусов задачи
export const TASK_STATUS_OPTIONS: TaskStatusOption[] = [
  { value: 'ToDo', label: 'К выполнению' },
  { value: 'InProgress', label: 'В работе' },
  { value: 'OnReview', label: 'На проверке' },
  { value: 'Done', label: 'Готово' }
];

// Словарь для быстрого получения подписи по enum с бэка
export const TASK_STATUS_LABELS: Record<TaskItemStatus, string> = {
  ToDo: 'К выполнению',
  InProgress: 'В работе',
  OnReview: 'На проверке',
  Done: 'Готово'
};
