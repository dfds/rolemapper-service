-- 2019-06-24 09:55:03 : initial_tables

CREATE table public."UnsureWhatDataToPersistYet" (
    "UndeterminedId" VARCHAR(255)
);

CREATE table public."DomainEvent" (
    "EventId" uuid NOT NULL,
    "AggregateId" varchar(255) NOT NULL,
    "Type" varchar(255) NOT NULL,
    "Format" varchar(255) NOT NULL,
    "Data" text NOT NULL,
    "Created" timestamp NOT NULL,
    "Sent" timestamp NULL,
    CONSTRAINT domainevent_pk PRIMARY KEY ("EventId")
);