-- One-off initial import.
-- Run from the repository root with:
-- docker exec -i <postgres-container> psql -U app_user -d app_db < initial-import.sql

BEGIN;

INSERT INTO contractors."Contractors" ("Id", "NIP", "Name", "Address_Street", "Address_City", "Address_ZipCode", "Email", "CreatedAt", "CreatedBy", "ModifiedAt", "ModifiedBy") VALUES ('00000000-0000-0000-0000-000000000001', '1234567890', 'Przykladowy Kontrahent Sp. z o.o.', 'Przykladowa 1', 'Warszawa', '00-001', 'demo.contractor@example.com', '2025-01-01 00:00:00+00', 'example-import', '2025-01-01 00:00:00+00', NULL);

INSERT INTO orders."ExcludedOrderItems" ("OrderId", "ItemNumber") VALUES ('EXAMPLE-ORDER-001', 'ITEM-001');

INSERT INTO orders."ExcludedOrders" ("Id") VALUES ('EXAMPLE-ORDER-002');

COMMIT;
