/**
 * Generated by orval v6.31.0 🍺
 * Do not edit manually.
 * Web
 * OpenAPI spec version: 1.0
 */
import type { OrderItem } from './orderItem';

export interface OrderDto {
  /** @nullable */
  customerNumber?: string | null;
  date?: string;
  /** @nullable */
  id?: string | null;
  isExcluded?: boolean;
  /** @nullable */
  items?: OrderItem[] | null;
  readonly itemsCount?: number;
  /** @nullable */
  number?: string | null;
  /** @nullable */
  person?: string | null;
  readonly totalPrice?: number;
}
