--liquibase formatted sql

--changeset your.name:1 labels:example-label context:example-context
--comment: example comment
CREATE TABLE currency_rates_changes
(
    id SERIAL PRIMARY KEY,
    selected_date date,
    currency_changes text
);
--rollback DROP TABLE currency_rates_changes;


