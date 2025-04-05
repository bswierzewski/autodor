/**
 * Generated by orval v6.31.0 🍺
 * Do not edit manually.
 * Web
 * OpenAPI spec version: 1.0
 */
import type { Contractor } from './contractor';
import type { OrderDto } from './orderDto';

export interface CreateInvoiceCommand {
  contractor?: Contractor;
  /** @nullable */
  invoiceNumber?: number | null;
  issueDate?: string;
  /** @minItems 1 */
  orders: OrderDto[];
  saleDate?: string;
}
