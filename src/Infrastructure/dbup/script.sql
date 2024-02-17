CREATE TABLE currency_rates(
	id serial PRIMARY KEY,
	currency VARCHAR(3),
	quantity int,
	rate DECIMAL,
	exchange_date DATE
	);