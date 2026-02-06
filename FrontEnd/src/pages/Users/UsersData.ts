export const UserStatus = [
    { label: 'Newly Created', value: '1' },
    { label: 'Active', value: '2' },
    { label: 'Locked', value: '3' },
    { label: 'Deleted', value: '4' }
];

export const UserStatusEnum: Record<string, number> = {
    NewlyCreated: 1,
    Active: 2,
    Locked: 3,
    Deleted: 4
};

// Helper: get status key from value
export const getStatusKey = (value: number): string => {
    return Object.keys(UserStatusEnum).find(key => UserStatusEnum[key] === value) ?? 'NewlyCreated';
};

// Helper: check user status
export const isNewlyCreated = (status: string | number): boolean => {
    if (typeof status === 'number') return status === UserStatusEnum.NewlyCreated;
    return status === 'NewlyCreated';
};

export const isActive = (status: string | number): boolean => {
    if (typeof status === 'number') return status === UserStatusEnum.Active;
    return status === 'Active';
};

export const isLocked = (status: string | number): boolean => {
    if (typeof status === 'number') return status === UserStatusEnum.Locked;
    return status === 'Locked';
};

export const isDeleted = (status: string | number): boolean => {
    if (typeof status === 'number') return status === UserStatusEnum.Deleted;
    return status === 'Deleted';
};

export const SortByOptions = [
    { label: 'Create Date', value: 'createdAt' },
    { label: 'UserName', value: 'userName' },
    { label: 'Email', value: 'email' },
];

export const SortDirOptions = [
    { label: 'Ascending', value: '1' },
    { label: 'Descending', value: '0' }
];

export const DETAIL_FIELD_MAP = {
  firstName: "FirstName",
  lastName: "LastName",
  phoneNumber: "PhoneNumber",
  birthDate: "BirthDate",
  address: "Address",
  location: "Location",
  timeZone: "TimeZone",
} as const;
