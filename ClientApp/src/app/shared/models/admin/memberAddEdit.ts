export interface MemberAddEdit {
  id?: string;
  userName: string;
  firstName: string;
  lastName: string;
  dateCreated: Date;
  password?: string;
  roles: string[];
}
