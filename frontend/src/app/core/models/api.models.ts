// Статусы задачи
export type TaskItemStatus = 'ToDo' | 'InProgress' | 'OnReview' | 'Done';

// Интерфейс пагинации для списков
export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

// Модель пользователя
export interface UserDto {
  id: string;
  email: string;
  name: string;
  createdAt: string;
}

// Модель проекта
export interface ProjectDto {
  id: string;
  name: string;
  description: string | null;
  createdByUserId: string | null;
  createdAt: string;
  tasksCount: number;
}

// Краткая модель задачи
export interface TaskItemSummaryDto {
  id: string;
  name: string;
  status: TaskItemStatus;
  projectId: string;
  projectName: string;
  assigneeId: string | null;
  assigneeName: string | null;
  createdAt: string;
}

// Детальная модель задачи
export interface TaskItemDetailsDto {
  id: string;
  name: string;
  description: string | null;
  status: TaskItemStatus;
  projectId: string;
  projectName: string;
  assigneeId: string | null;
  assigneeName: string | null;
  assigneeEmail: string | null;
  createdAt: string;
}

// Токен доступа
export interface AccessTokenDto {
  accessToken: string;
}

// Ответ при успешном логине/регистрации
export interface AuthResponseDto {
  user: UserDto;
  accessToken: string;
}

// Request-модели

export interface RegisterRequest {
  email: string;
  password: string;
  name: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface CreateUserRequest {
  email: string;
  password: string;
  name: string;
}

export interface UpdateUserNameRequest {
  newName: string;
}

export interface ChangeUserPasswordRequest {
  currentPassword: string;
  newPassword: string;
}

export interface CreateProjectRequest {
  name: string;
  description?: string | null;
}

export interface PatchProjectRequest {
  name: string;
  description?: string | null;
}

export interface CreateTaskItemRequest {
  name: string;
  projectId: string;
  description?: string | null;
  assigneeId?: string | null;
  status?: TaskItemStatus;
}

export interface PatchTaskItemRequest {
  name: string;
  description?: string | null;
  projectId: string;
  assigneeId?: string | null;
  status: TaskItemStatus;
}

export interface ChangeTaskItemStatusRequest {
  status: TaskItemStatus;
}

export interface AssignTaskItemRequest {
  assigneeId: string | null;
}

// Модели ошибок бэкенда (ProblemDetails)
export interface ProblemDetails {
  type?: string | null;
  title?: string | null;
  status?: number | null;
  detail?: string | null;
  instance?: string | null;
}

export interface HttpValidationProblemDetails extends ProblemDetails {
  errors?: Record<string, string[]>;
}
