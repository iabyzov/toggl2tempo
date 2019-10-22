import * as moment from "moment";

export const SIMPLE_DATE_TIME_FORMAT = "YYYY-MM-DD HH:mm:ss.SSS";
const ISO_DATE_TIME_WITHOUT_TIMEZONE_FORMAT = "YYYY-MM-DDTHH:mm:ss.SSS";
const CSHARP_DATE_TIME_TO_STRING_FORMAT = "MM/DD/YYYY HH:mm:ss";

export class DateUtils {
    /**
     * Get current date
     *
     * @static
     * @returns {Date} current date
     *
     * @memberOf DateUtils
     */
    public static now(): Date {
        return moment().toDate();
    }

    /**
     * Get current UTC date
     *
     * @static
     * @returns {Date} current date in UTC
     *
     * @memberOf DateUtils
     */
    public static utcNow(): Date {
        return moment().utc().toDate();
    }

    /**
     * Compare 2 dates and return the difference in seconds
     *
     * @static
     * @param {Date} source date for substraction
     * @param {Date} subtrahend date, which will be substracted from the source
     * @returns {number} difference between the dates in seconds
     *
     * @memberOf DateUtils
     */
    public static diffSeconds(source: Date, subtrahend:Date): number {
        const seconds =  moment(source).diff(moment(subtrahend), "seconds");
        return seconds;
    }

    /**
     * Add seconds to date
     *
     * @static
     * @param {Date} source date to add seconds
     * @param {number} seconds how many seconds you want to add
     * @returns {Date} result
     *
     * @memberOf DateUtils
     */
    public static addSeconds(source: Date, seconds: number): Date {
        return  moment(source).add(seconds, "seconds").toDate();
    }

    public static getAge(dob: Date, endDate: Date): number {
        return DateUtils.getYears(dob, endDate);
    }

    public static getYears(startDate: Date, endDate: Date): number {
        var value = moment(endDate)
            .diff(moment(startDate), "years");

        return value;
    }

    public static monthDiff(startDate: Date, endDate: Date): number {
        var value = moment(endDate)
            .diff(moment(startDate), "months");

        return value;
    }

    public static isLeapYear(year: number): boolean {
        return moment([year])
            .isLeapYear();
    }

    public static getDaysInMonth(year: number, month: number): number {
        var sourceDate = new Date(year, month);
        var value = moment(sourceDate).daysInMonth();

        return value;
    }

    public static addMonths(date: Date, months: number): Date {
        return moment(date)
            .add(months, "months")
            .toDate();
    }

    public static addYears(date: Date, years: number): Date {
        return moment(date)
            .add(years, "years")
            .toDate();
    }

    public static getDateWithoutTime(date: Date): Date {
        var d = new Date(+date);
        d.setHours(0, 0, 0, 0);
        return d;
    }

    public static createDateIgnoringTimezone(dateString: string): Date {
        return moment(dateString).toDate();
    }

    public static toUrlFriendlyIsoDateTimeIgnoringTimezone(value: Date): string {
        if (!value) {
            return null;
        }

        return moment(value).format("YYYY-MM-DDTHHmmss");
    }

    public static toIsoDateTimeStringIgnoringTimezone(value: Date): string {
        if (!value) {
            return null;
        }

        return moment(value).format(ISO_DATE_TIME_WITHOUT_TIMEZONE_FORMAT);
    }

    public static toIsoDateStringIgnoringTimezone(value: Date): string {
        if (!value) {
            return null;
        }

        let date = DateUtils.getDateWithoutTime(value);

        return moment(date).format("YYYY-MM-DD");
    }

    public static toDateStringIgnoringTimezone(value: Date): string {
        if (!value) {
            return null;
        }

        return moment(value).format("MM/DD/YYYY");
    }

    public static createDateTimeString(timestamp: number): string {
        return moment.utc(timestamp).format(SIMPLE_DATE_TIME_FORMAT);
    }

    public static dateTimeToSimpleString(dateTime: Date): string {
        return moment(dateTime).format(SIMPLE_DATE_TIME_FORMAT);
    }

    public static withUtcOffset(dateTime: string, offset: number): string {
        return moment.utc(dateTime).utcOffset(offset).format(SIMPLE_DATE_TIME_FORMAT);
    }

    public static getShift(from: string, to: string): number {
        return moment.utc(to).diff(moment.utc(from)).valueOf();
    }

    public static toDateWith12PM(from: Date): Date {
        return new Date(from.getFullYear(),
                        from.getMonth(),
                        from.getDate(),
                        12, 0, 0, 0);
    }

    public static parseUtcFromServer(source: string): Date {
        return moment.utc(source, CSHARP_DATE_TIME_TO_STRING_FORMAT).toDate();
    }
}