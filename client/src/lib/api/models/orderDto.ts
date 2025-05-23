/**
 * Generated by orval v7.8.0 🍺
 * Do not edit manually.
 * Web
 * OpenAPI spec version: 1.0
 */
import type { OrderItem } from './orderItem';

export interface OrderDto {
  isExcluded?: boolean;
  date?: string;
  /** @nullable */
  id?: string | null;
  /** @nullable */
  number?: string | null;
  /** @nullable */
  person?: string | null;
  /** @nullable */
  customerNumber?: string | null;
  /** @nullable */
  items?: OrderItem[] | null;
  readonly itemsCount?: number;
  readonly totalPrice?: number;
}
